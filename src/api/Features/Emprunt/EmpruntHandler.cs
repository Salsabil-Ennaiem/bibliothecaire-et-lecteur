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
    var allemprunts = await _empruntsRepository.GetAllEmpAsync();
    var emprunts = allemprunts.Where(e => e.Statut_emp == Statut_emp.en_cours);
    var dateNow = DateTime.UtcNow.Date;
    var param = await _parametreRepository.GetParam();

    foreach (var emp in emprunts)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var emailBody = param.Modele_Email_Retard ??
                $"URGENT : {emp.prenom} {emp.nom} Votre Emprunts avec ID {emp.id_emp} est passé la date prévue : {emp.date_retour_prevu:d}. Donc votre livre {emp.titre} est en retard, merci de le retourner rapidement.";

            emailBody = emailBody.Replace("Id Emprunt", emp.id_emp.ToString(), StringComparison.OrdinalIgnoreCase)
                                 .Replace("Date Emprunt", emp.date_emp.ToString("d"), StringComparison.OrdinalIgnoreCase)
                                 .Replace("Titre", emp.titre, StringComparison.OrdinalIgnoreCase)
                                 .Replace("nom", emp.nom, StringComparison.OrdinalIgnoreCase)
                                 .Replace("prenom", emp.prenom, StringComparison.OrdinalIgnoreCase)
                                 .Replace("Date", emp.date_retour_prevu.ToString("d"), StringComparison.OrdinalIgnoreCase);

            string messageNotif = $"{emp.prenom} {emp.nom} qui est {emp.cin_ou_passeport} emprunte Livre '{emp.titre}' doit le rendre.";
            var datePrevu = emp.date_retour_prevu;

            if (datePrevu == dateNow.AddDays(1))
            {
                await EnvoyerNotificationAsync(emp.id_emp, messageNotif, "demain");
            }
            else if (datePrevu == dateNow)
            {
                await EnvoyerNotificationAsync(emp.id_emp, messageNotif, "aujourd'hui");
                // Separate email sending from transaction
                await SendEmailAsync(emp.email, "Retour de livre Aujourd'hui", emailBody);
            }
            else if (datePrevu < dateNow)
            {
                await SendEmailAsync(emp.email, "Retour de livre en retard", emailBody);
            }
            else if (datePrevu < dateNow.AddYears(1))
            {
                emp.Statut = StatutMemb.block;
                emp.Statut_emp = Statut_emp.perdu;
                var inventaire = await _dbContext.Inventaires.FirstOrDefaultAsync(l => l.id_inv == emp.id_inv);
                if (inventaire != null)
                {
                    inventaire.statut = Statut_liv.perdu;
                }
            }

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            // Log exception as needed but avoid swallowing it
            throw new Exception("Error notification: " + ex.Message, ex);
        }
    }
}
   public async Task<EmppruntDTO> CreateAsync(string id, CreateEmpRequest empdto)
    {
        return await _empruntsRepository.CreateAsync(id, empdto);

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
        if (searchTerm ==null) { return list; }
        var query = list.Where(e => (e.id_membre == searchTerm)
                            || (e.cote_liv != null && e.cote_liv.Contains(searchTerm))
                            || (e.note != null && e.note.Contains(searchTerm))
                            || (e.nom != null && e.nom.Contains(searchTerm))
                            || (e.prenom != null && e.prenom.Contains(searchTerm))
                            || (e.email != null && e.email.Contains(searchTerm))
                            || e.date_emp.ToString().Contains(searchTerm)
                            || (e.titre != null && e.titre.Contains(searchTerm))
                            || (e.cin_ou_passeport != null && e.cin_ou_passeport.Contains(searchTerm)));
        return query;
    }
    public async Task<IEnumerable<EmppruntDTO>> FiltreStautEmp(Statut_emp? statut_emp)
    {
        var emp = await GetAllAsync();
        if(statut_emp==null)
            return emp;
        return emp.Where(r => r.Statut_emp == statut_emp);
    }
 
}

