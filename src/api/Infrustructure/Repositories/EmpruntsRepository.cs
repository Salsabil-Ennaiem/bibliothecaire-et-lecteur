using api.Features.Emprunt;
using Data;
using domain.Entity;
using domain.Entity.Enum;
using domain.Interfaces;
using Infrastructure.Repositories;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositries
{
    public class EmpruntsRepository : IEmpruntsRepository
    {
        private readonly BiblioDbContext _dbContext;
        private readonly IRepository<Membre> _membreRepository;
        private readonly LivresRepository _LivresRepository;
        private readonly IParametreRepository _ParametreRepository;

        public EmpruntsRepository(BiblioDbContext dbContext, IParametreRepository ParametreRepository, IRepository<Membre> membreRepository, LivresRepository LivresRepository)

        {
            _dbContext = dbContext;
            _membreRepository = membreRepository;
            _ParametreRepository = ParametreRepository;
        }
        public async Task<IEnumerable<EmppruntDTO>> GetAllEmpAsync()
        {
            return await (from e in _dbContext.Emprunts
                          join m in _dbContext.Membres on e.id_membre equals m.id_membre
                          join i in _dbContext.Inventaires on e.Id_inv equals i.id_inv
                          join l in _dbContext.Livres on i.id_liv equals l.id_livre
                          select new EmppruntDTO
                          {
                              cote_liv = i.cote_liv,
                              id_emp = e.id_emp,
                              editeur = l.editeur,
                              id_inv = i.id_inv,
                              date_edition = l.date_edition,
                              titre = l.titre,
                              date_emp = e.date_emp,
                              // date_retour_prevu = e.date_retour_prevu,
                              date_effectif = e.date_effectif,
                              Statut_emp = e.Statut_emp,
                              note = e.note,
                              TypeMembre = m.TypeMembre,
                              nom = m.nom,
                              prenom = m.prenom,
                              email = m.email,
                              telephone = m.telephone,
                              cin_ou_passeport = m.cin_ou_passeport,
                              Statut = m.Statut
                          }).ToListAsync();
        }
        public async Task<EmppruntDTO> GetByIdAsync(string id)
        {
            try
            {
                var query = from e in _dbContext.Emprunts
                            join m in _dbContext.Membres on e.id_membre equals m.id_membre
                            join i in _dbContext.Inventaires on e.Id_inv equals i.id_inv
                            join l in _dbContext.Livres on i.id_liv equals l.id_livre
                            where e.id_emp == id
                            select new EmppruntDTO
                            {
                                id_emp = e.id_emp,
                                cote_liv = i.cote_liv,
                                editeur = l.editeur,
                                id_inv = i.id_inv,
                                date_edition = l.date_edition,
                                titre = l.titre,
                                date_emp = e.date_emp,
                                date_retour_prevu = e.date_retour_prevu ?? default(DateTime),
                                date_effectif = e.date_effectif,
                                Statut_emp = e.Statut_emp,
                                note = e.note,
                                TypeMembre = m.TypeMembre,
                                nom = m.nom,
                                prenom = m.prenom,
                                email = m.email,
                                telephone = m.telephone,
                                cin_ou_passeport = m.cin_ou_passeport,
                                Statut = m.Statut
                            };
                if (query is null)
                {
                    throw new Exception($"{query} with ID {id} not found");
                }

                return await query.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving Livre with ID {id}: {ex.Message}", ex);
            }
        }
        public async Task<Emprunts?> GetEmpByIdAsync(string empid)
        {
            try
            {
                return await _dbContext.Emprunts
                    .Include(e => e.Membre)
                    .FirstOrDefaultAsync(i => i.id_emp == empid);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting Livre with ID {empid}: {ex.Message}", ex);
            }
        }
        public async Task DeleteAsync(string id)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {

                var emprunts = await GetEmpByIdAsync(id);
                var MembreId = emprunts.id_membre;
                if (emprunts.Statut_emp != Statut_emp.retourne)
                {
                    throw new Exception("Vous n'avez pas le droit de supprimer un emprunt en cours ou perdu");
                }
                _dbContext.Emprunts.Remove(emprunts);

                bool hasOtherEmp = await _dbContext.Emprunts
                                 .AnyAsync(e => e.id_membre == MembreId && e.id_emp != id);

                // If no other copies, remove the Livres itself
                if (!hasOtherEmp)
                {
                    var livre = await _dbContext.Livres.FindAsync(MembreId);
                    if (livre != null)
                    {
                        _dbContext.Livres.Remove(livre);
                    }
                }
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                Console.WriteLine($"Emprunts with ID {id} deleted successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error deleting Emprunts with ID {id}: {ex.Message}", ex);
            }
        }
        public async Task<IEnumerable<Emprunts>> GetOverdueEmpruntsAsync(string userId, DateTime currentDate)
        {
            return await _dbContext.Emprunts
                .Where(e => e.id_biblio == userId
                         && e.date_retour_prevu > currentDate
                         && (e.date_effectif == null || e.Statut_emp != Statut_emp.retourne)) // not returned
                .ToListAsync();
        }
        public async Task<EmppruntDTO> CreateAsync(CreateEmpRequest empdto)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                // 1. Recherche du membre existant via le repository générique
                var allMembres = await _membreRepository.GetAllAsync();
                var membreExistant = allMembres.FirstOrDefault(m =>
                    (!string.IsNullOrEmpty(empdto.cin_ou_passeport) && m.cin_ou_passeport == empdto.cin_ou_passeport) ||
                    (!string.IsNullOrEmpty(empdto.email) && m.email == empdto.email)
                );
                if (membreExistant != null && membreExistant.Statut != StatutMemb.actif)
                {
                    if (membreExistant.Statut == StatutMemb.sanctionne)
                    {
                        //add serach sanction how time he gonna be wait until he finish that
                        throw new Exception("this membre not allowed to get any book right now ");
                    }
                    else
                        throw new Exception("this membre not allowed to have any service with us  ");
                }
                // 2. Création du membre si inexistant
                else if (membreExistant == null)
                {
                    var nouveauMembre = new Membre
                    {
                        id_membre = Guid.NewGuid().ToString(),
                        nom = empdto.nom,
                        prenom = empdto.prenom,
                        cin_ou_passeport = empdto.cin_ou_passeport,
                        email = empdto.email,
                        telephone = empdto.telephone,
                        TypeMembre = empdto.TypeMembre
                    };
                    membreExistant = await _membreRepository.CreateAsync(nouveauMembre);
                }
                var s = _LivresRepository.RechercheCote(empdto.cote_liv);
                var delais = await _ParametreRepository.GetDelais(empdto.TypeMembre);

                var nouveauEmp = new Emprunts
                      {
                          id_emp = Guid.NewGuid().ToString(),
                          Id_inv = s,
                          date_retour_prevu = DateTime.Now.AddDays(Convert.ToDouble(delais))
                      };
                       await _dbContext.Emprunts.AddAsync(nouveauEmp);
                       await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                return empdto.Adapt<EmppruntDTO>();

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Error creating book and inventory: " + ex.Message, ex);
            }
        }
    }
}