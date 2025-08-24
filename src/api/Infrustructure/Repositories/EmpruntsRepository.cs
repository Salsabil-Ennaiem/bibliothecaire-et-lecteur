using api.Features.Emprunt;
using Data;
using domain.Entity;
using domain.Entity.Enum;
using domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositries
{
    public class EmpruntsRepository : IEmpruntsRepository
    {
        private readonly BiblioDbContext _dbContext;

        public EmpruntsRepository(BiblioDbContext dbContext)

        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<EmppruntDTO>> GetAllEmpAsync()
        {
            return await (from e in _dbContext.Emprunts
                          join m in _dbContext.Membres on e.id_membre equals m.id_membre
                          join i in _dbContext.Inventaires on e.Id_inv equals i.id_inv
                          join l in _dbContext.Livres on i.id_liv equals l.id_livre
                          select new EmppruntDTO
                          {
                              id_emp = e.id_emp,
                              editeur=l.editeur,
                              id_inv=i.id_inv,
                              date_edition=l.date_edition,
                              titre=l.titre,
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
                            join m in _dbContext.Membres
                            on e.id_membre equals m.id_membre
                            where e.id_emp == id
                            select new EmppruntDTO
                            {
                                id_emp = e.id_emp,
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

                _dbContext.Emprunts.Remove(emprunts);

                bool hasOtherCopies = await _dbContext.Membres
                                 .AnyAsync(e => e.id_membre == MembreId);

                // If no other copies, remove the Livres itself
                if (!hasOtherCopies)
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

    }
}