using System.Security.Claims;
using domain.Entity;
using domain.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.AspNetCore.Identity;
using Data;
using domain.Entity.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using Infrastructure.SignalR;
using System.Text.RegularExpressions;

namespace api.Features.Emprunt;

public class EmpruntHandler
{
    private readonly IEmpruntsRepository _empruntsRepository;
    private readonly IParametreRepository _parametreRepository;
    private readonly UserManager<Bibliothecaire> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;
    private readonly BiblioDbContext _dbContext;
    private readonly IHubContext<NotificationHub> _hubContext;


    public EmpruntHandler(BiblioDbContext dbContext, IHubContext<NotificationHub> hubContext, IHttpContextAccessor httpContextAccessor, UserManager<Bibliothecaire> userManager, IEmpruntsRepository empruntsRepository, IParametreRepository parametreRepository, IConfiguration configuration)
    {
        _empruntsRepository = empruntsRepository;
        _parametreRepository = parametreRepository;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
        _hubContext = hubContext;
        _dbContext = dbContext;
    }
    private async Task EnvoyerNotificationAsync(string userId, string message, string when)
    {
        // Ici userId doit être identifiant SignalR ou groupe auquel l'utilisateur appartient
        await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", message, when);
    }
    private async Task SendEmailAsync(string userEmail, string subject, string body)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress(_configuration["MailSettings:Username"], _configuration["MailSettings:From"]));
        emailMessage.To.Add(new MailboxAddress("", userEmail));
        emailMessage.Subject = subject;
        emailMessage.Body = new TextPart("plain") { Text = body };

        using var client = new SmtpClient();
        await client.ConnectAsync(_configuration["MailSettings:Host"], int.Parse(_configuration["MailSettings:Port"]), MailKit.Security.SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_configuration["MailSettings:Username"], _configuration["MailSettings:Password"]);
        await client.SendAsync(emailMessage);
        await client.DisconnectAsync(true);
    }
    public async Task GererAlertesEtNotificationsAsync()
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var allemprunts = await _empruntsRepository.GetAllEmpAsync();
            var emprunts = allemprunts.Where(e => e.Statut_emp == Statut_emp.en_cours);
            var dateNow = DateTime.UtcNow.Date;
            var param = await _parametreRepository.GetParam();


            foreach (var emp in emprunts)
            {
                var emailBody = param.Modele_Email_Retard ??
                    $"URGENT : {emp.prenom} {emp.nom} Votre Emprunts avec ID {emp.id_emp} est passé la date prévue : {emp.date_retour_prevu:d}. Donc votre livre {emp.titre} est en retard, merci de le retourner rapidement.";

                emailBody = Regex.Replace(emailBody, @"Id\s*Emprunt", emp.id_emp, RegexOptions.IgnoreCase);
                emailBody = Regex.Replace(emailBody, @"Date\s*Emprunt", emp.date_emp.ToString("d"), RegexOptions.IgnoreCase);
                emailBody = Regex.Replace(emailBody, @"Titre", emp.titre, RegexOptions.IgnoreCase);
                emailBody = Regex.Replace(emailBody, @"nom", emp.nom, RegexOptions.IgnoreCase);
                emailBody = Regex.Replace(emailBody, @"prenom", emp.prenom, RegexOptions.IgnoreCase);
                emailBody = Regex.Replace(emailBody, @"Date", emp.date_retour_prevu.ToString("d"), RegexOptions.IgnoreCase);

                string messageNotif = $"{emp.prenom} {emp.nom} qui est {emp.cin_ou_passeport} emprnte Livre '{emp.titre}' doit le rendre .";
                var datePrevu = emp.date_retour_prevu;

                if (datePrevu == dateNow.AddDays(1))
                {
                    // Notification simple (badge)
                    await EnvoyerNotificationAsync(emp.id_emp, messageNotif, "demain");
                }
                else if (datePrevu == dateNow)
                {
                    // Notification + Email
                    await EnvoyerNotificationAsync(emp.id_emp, messageNotif, "aujourd'hui");
                    await SendEmailAsync(emp.email, "Retour de livre Aujourd'hui", emailBody);

                }

                else if (datePrevu < dateNow.AddDays(7))
                {
                    //Email
                    await SendEmailAsync(emp.email, "Retour de livre en retard", emailBody);
                }
                else if (datePrevu < dateNow.AddYears(1))
                {
                    // Blocage + état perdu
                    emp.Statut = StatutMemb.block;
                    emp.Statut_emp = Statut_emp.perdu;
                    var Inventaires = await _dbContext.Inventaires.FirstOrDefaultAsync(l => l.id_inv == emp.id_inv);
                    Inventaires.statut = Statut_liv.perdu;
                    await _dbContext.SaveChangesAsync();
                }
                await transaction.CommitAsync();
            }
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new Exception("Error notification: " + ex.Message, ex);
        }
    }
    public async Task<EmppruntDTO> CreateAsync(CreateEmpRequest empdto)
    {
        return await _empruntsRepository.CreateAsync(empdto);

    }
    public async Task<IEnumerable<EmppruntDTO>> GetAllAsync()
    {
        return await _empruntsRepository.GetAllEmpAsync();
    }
    public async Task<EmppruntDTO> GetByIdAsync(string id)
    {
        return await _empruntsRepository.GetByIdAsync(id);
    }
    public async Task<EmppruntDTO> UpdateAsync(string id, UpdateEmppruntDTO emp)
    {
        return await _empruntsRepository.UpdateAsync(id, emp);
    }
    public async Task DeleteAsync(string id)
    {
        await _empruntsRepository.DeleteAsync(id);
    }
    public async Task<IEnumerable<EmppruntDTO>> SearchAsync(string searchTerm)
    {
        var list = await _empruntsRepository.GetAllEmpAsync();
                        if(searchTerm=="") { return list;}
        var query = list.Where(e => e.date_emp.ToString().Contains(searchTerm)
                            || (e.cote_liv != null && e.cote_liv.Contains(searchTerm))
                            || (e.note != null && e.note.Contains(searchTerm))
                            || (e.nom != null && e.nom.Contains(searchTerm))
                            || (e.prenom != null && e.prenom.Contains(searchTerm))
                            || (e.email != null && e.email.Contains(searchTerm))
                           // || e.Statut_emp.ToString().Contains(searchTerm)
                            || (e.titre != null && e.titre.Contains(searchTerm))
                            || (e.cin_ou_passeport != null && e.cin_ou_passeport.Contains(searchTerm)));
        return query;
    }    
    private string GetCurrentUserId()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("User is not authenticated.");
        return userId;
    }
    /* public async Task ImportAsync(Stream excelStream)
     {
         var workbook = new XSSFWorkbook(excelStream);
         var sheet = workbook.GetSheetAt(0);

         for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++)
         {
             var row = sheet.GetRow(rowIndex);
             if (row == null) continue;

             var emp = new Emprunts
             {
                 id_emp = Guid.NewGuid().ToString(),
                 id_membre = row.GetCell(0)?.StringCellValue,
                 id_biblio = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                 Id_inv = row.GetCell(1)?.StringCellValue,
                 date_emp = DateTime.UtcNow,
                 date_retour_prevu = row.GetCell(2)?.DateCellValue,
                 date_effectif = row.GetCell(3)?.DateCellValue,
                 Statut_emp = Enum.Parse<Statut_emp>(row.GetCell(4)?.StringCellValue ?? "en_cours"),
                 note = row.GetCell(5)?.StringCellValue
             };

             await _empruntsRepository.CreateAsync(emp);
         }
     }
     */
    /* public async Task<MemoryStream> ExportAsync()
     {
         var data = await SearchAsync(""); // Get all data

         var workbook = new XSSFWorkbook();
         var sheet = workbook.CreateSheet("Emprunts");

         var headerRow = sheet.CreateRow(0);
         string[] headers = {
             "IdEmprunt", "IdMembre", "IdBibliothecaire", "IdInventaire", "DateEmprunt",
             "DateRetourPrevu", "DateEffectif", "StatutEmprunt", "Note"
         };

         for (int i = 0; i < headers.Length; i++)
             headerRow.CreateCell(i).SetCellValue(headers[i]);

         int rowIndex = 1;
         foreach (var emprunt in data)
         {
             var row = sheet.CreateRow(rowIndex++);
             row.CreateCell(0).SetCellValue(emprunt.id_emp ?? "");
             row.CreateCell(1).SetCellValue(emprunt.id_membre ?? "");
             row.CreateCell(2).SetCellValue(emprunt.id_biblio ?? "");
             row.CreateCell(3).SetCellValue(emprunt.Id_inv ?? "");
             row.CreateCell(4).SetCellValue(emprunt.date_emp.ToString());
             row.CreateCell(5).SetCellValue(emprunt.date_retour_prevu?.ToString() ?? "");
             row.CreateCell(6).SetCellValue(emprunt.date_effectif?.ToString() ?? "");
             row.CreateCell(7).SetCellValue(emprunt.Statut_emp.ToString());
             row.CreateCell(8).SetCellValue(emprunt.note ?? "");
         }

         var stream = new MemoryStream();
         workbook.Write(stream);
         stream.Position = 0;
         return stream;
     }
 */
}

