using api.Features.Emprunt;
using api.Features.Membres;
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
        private readonly MembreHandler _membreRepository;
        private readonly ILivresRepository _LivresRepository;
        private readonly IParametreRepository _ParametreRepository;


        public EmpruntsRepository(BiblioDbContext dbContext, IParametreRepository ParametreRepository, MembreHandler membreRepository, ILivresRepository LivresRepository)

        {
            _dbContext = dbContext;
            _membreRepository = membreRepository;
            _ParametreRepository = ParametreRepository;
            _LivresRepository = LivresRepository;
        }
        public async Task<EmppruntDTO> CreateAsync(string id_inv, CreateEmpRequest empdto)
        {
            try
            {
                if (string.IsNullOrEmpty(empdto.cin_ou_passeport) || string.IsNullOrEmpty(empdto.email))
                {
                    throw new Exception(" CIN , email doit etre remplir au minimum");
                }
                // 1. Recherche du membre existant
                var allMembres = await _membreRepository.GetAllMembAsync();
                var membreExistant = allMembres.FirstOrDefault(m =>
                    (!string.IsNullOrEmpty(empdto.cin_ou_passeport) && m.cin_ou_passeport == empdto.cin_ou_passeport) ||
                    (!string.IsNullOrEmpty(empdto.email) && m.email == empdto.email)
                );
                if (membreExistant != null && membreExistant.Statut != StatutMemb.actif)
                {
                    if (membreExistant.Statut == StatutMemb.sanctionne)
                    {
                        throw new Exception("this membre not allowed to get any book right now ");
                    }
                    else
                        throw new Exception("this membre not allowed to have any service with us  ");
                }
                // 2. Création du membre si inexistant
                else if (membreExistant == null)
                {
                    var cin = _membreRepository.SearchAsync(empdto.cin_ou_passeport);
                    var email = _membreRepository.SearchAsync(empdto.email);
                    if (cin == null && email == null)
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
                        await _dbContext.Membres.AddAsync(nouveauMembre);

                        membreExistant = nouveauMembre.Adapt<MembreDto>();
                    }
                    else  throw new Exception("cin ou email deja existe soit changer le ou ecriver tous donne correcte");

                }
                var delais = await _ParametreRepository.GetDelais(empdto.TypeMembre);

                var nouveauEmp = new Emprunts
                {
                    id_emp = Guid.NewGuid().ToString(),
                    id_membre = membreExistant.id_membre,
                    Id_inv = id_inv,
                    date_retour_prevu = DateTime.UtcNow.AddDays(Convert.ToDouble(delais))
                };
                var liv = await _LivresRepository.GetByIdAsync(id_inv);
                liv.statut = Statut_liv.emprunte;
                var entity = liv.Adapt<Inventaire>();
                _dbContext.Inventaires.Update(entity);
                await _dbContext.Emprunts.AddAsync(nouveauEmp);
                await _dbContext.SaveChangesAsync();
                return empdto.Adapt<EmppruntDTO>();

            }
            catch (Exception ex)
            {
                throw new Exception("Error  " + ex.Message, ex);
            }
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
                              date_retour_prevu = e.date_retour_prevu,
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
                                date_retour_prevu = e.date_retour_prevu,
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

                var result = await query.FirstOrDefaultAsync();

                if (result == null)
                {
                    throw new Exception($"Empprunt with ID {id} not found");
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving Empprunt with ID {id}: {ex.Message}", ex);
            }
        }
        public async Task<EmppruntDTO> UpdateAsync(string id, UpdateEmppruntDTO updateEmpReq)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                // Fetch the tracked entity from the database for update
                var empEntity = await _dbContext.Emprunts.FindAsync(id);

                if (empEntity == null)
                {
                    throw new Exception($"Empprunt entity with ID {id} not found");
                }

                // Update the tracked entity properties as requested
                if (updateEmpReq.Statut_emp != Statut_emp.en_cours)
                {
                    empEntity.Statut_emp = updateEmpReq.Statut_emp;

                    if (updateEmpReq.Statut_emp == Statut_emp.retourne)
                    {
                        empEntity.date_effectif = DateTime.Now;
                        var liv = await _LivresRepository.GetByIdAsync(empEntity.Id_inv);
                        liv.statut = Statut_liv.disponible;
                        var entity = liv.Adapt<Livres>();
                        _dbContext.Livres.Update(entity);
                    }
                }

                if (!string.IsNullOrEmpty(updateEmpReq.note))
                {
                    empEntity.note = updateEmpReq.note;
                }

                _dbContext.Entry(empEntity).State = EntityState.Modified;

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                // Return updated DTO by reloading data with joins for enriched info
                var updatedDto = await GetByIdAsync(id);

                return updatedDto;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error updating Empprunt with ID {id}: {ex.Message}", ex);
            }
        }
        public async Task DeleteAsync(string id)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var emprunts = await GetEmpByIdAsync(id);
                if (emprunts.Statut_emp == Statut_emp.retourne)
                {
                    var MembreId = emprunts.id_membre;
                    if (emprunts.Statut_emp != Statut_emp.retourne)
                    {
                        throw new Exception("Vous n'avez pas le droit de supprimer un emprunt en cours ou perdu");
                    }
                    _dbContext.Emprunts.Remove(emprunts);
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    Console.WriteLine($"Emprunts with ID {id} deleted successfully.");
                }
                else { throw new Exception($"Error deleting Emprunts with ID {id}: l'emprunts  doit etre retourne  "); }

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error deleting Emprunts with ID {id}: {ex.Message}", ex);
            }
        }
        private async Task<Emprunts?> GetEmpByIdAsync(string empid)
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

    }
}