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
        /*
          public async Task<LivreDTO> CreateAsync(CreateLivreRequest livreCreate)
                {
                    using var transaction = await _dbContext.Database.BeginTransactionAsync();
                    try
                    {
                        if (string.IsNullOrEmpty(livreCreate.titre) ||
                         string.IsNullOrEmpty(livreCreate.editeur) ||
                         string.IsNullOrEmpty(livreCreate.date_edition) ||
                         string.IsNullOrEmpty(livreCreate.cote_liv) )
                        {
                            throw new Exception("doit remplir les 4 champs sont obligatoire :titre , editeur , date edition , cote liv ");
                        }
                        // Check if book exists by title & edition
                        var existingLivre = await _dbContext.Livres
                            .FirstOrDefaultAsync(l => l.titre == livreCreate.titre && l.date_edition == livreCreate.date_edition);

                        if (existingLivre != null)
                        {
                            // Book exists: check if the inventory cote_liv already exists
                            var inventaireExists = await _dbContext.Inventaires
                                .AnyAsync(i => i.cote_liv == livreCreate.cote_liv);

                            if (inventaireExists)
                            {
                                throw new Exception("Failed to add book: inventory cote_liv already exists");
                            }


                            // Add new inventory linked to existing book
                            var newInventaire = new Inventaire
                            {
                                id_inv = Guid.NewGuid().ToString(),
                                id_liv = existingLivre.id_livre,
                                cote_liv = livreCreate.cote_liv,
                                etat = livreCreate.etat,
                                inventaire = livreCreate.inventaire
                            };

                            await _dbContext.Inventaires.AddAsync(newInventaire);
                            await _dbContext.SaveChangesAsync();

                            await transaction.CommitAsync();
                            throw new Exception("Inventory added to existing book successfully");
                        }
                        else
                        {
                            // Book does not exist: create it
                            var newLivre = new Livres
                            {
                                id_livre = Guid.NewGuid().ToString(),
                                titre = livreCreate.titre,
                                date_edition = livreCreate.date_edition,
                                auteur = livreCreate.auteur,
                                isbn = livreCreate.isbn,
                                editeur = livreCreate.editeur,
                                Description = livreCreate.Description,
                                Langue = livreCreate.Langue,
                                couverture = livreCreate.couverture,
                            };

                            await _dbContext.Livres.AddAsync(newLivre);
                            await _dbContext.SaveChangesAsync();

                            // Create inventory linked to new book
                            var newInventaire = new Inventaire
                            {
                                id_inv = Guid.NewGuid().ToString(),
                                id_liv = newLivre.id_livre,
                                cote_liv = livreCreate.cote_liv,
                                etat = livreCreate.etat,
                                inventaire = livreCreate.inventaire
                            };

                            await _dbContext.Inventaires.AddAsync(newInventaire);
                            await _dbContext.SaveChangesAsync();

                            await transaction.CommitAsync();
                            return newLivre.Adapt<LivreDTO>();
                        }
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw new Exception("Error creating book and inventory: " + ex.Message, ex);
                    }
                }
                public async Task<LivreDTO> UpdateAsync(string id, UpdateLivreDTO updatelivReq)
                {
                    using var transaction = await _dbContext.Database.BeginTransactionAsync();
                    try
                    {

                        var inventaire = await GetInventaireByIdAsync(id);
                        var livre = inventaire.Livre;

                        if (updatelivReq.titre is not null)
                            livre.titre = updatelivReq.titre;

                        if (updatelivReq.auteur is not null)
                            livre.auteur = updatelivReq.auteur;

                        if (updatelivReq.cote_liv is not null)
                            inventaire.cote_liv = updatelivReq.cote_liv;

                        if (updatelivReq.couverture is not null)
                            livre.couverture = updatelivReq.couverture;

                        if (updatelivReq.date_edition is not null)
                            livre.date_edition = updatelivReq.date_edition;

                        if (updatelivReq.Description is not null)
                            livre.Description = updatelivReq.Description;

                        if (updatelivReq.editeur is not null)
                            livre.editeur = updatelivReq.editeur;

                        if (updatelivReq.etat is not null)
                            inventaire.etat = updatelivReq.etat;

                        if (updatelivReq.isbn is not null)
                            livre.isbn = updatelivReq.isbn;

                        if (updatelivReq.Langue is not null)
                            livre.Langue = updatelivReq.Langue;

                        if (updatelivReq.statut != Statut_liv.disponible)
                            inventaire.statut = updatelivReq.statut;

                        if (updatelivReq.inventaire is not null)
                            inventaire.inventaire = updatelivReq.inventaire;

                        await _dbContext.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return await GetByIdAsync(id);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw new Exception($"Error updating Livre with ID {id}: {ex.Message}", ex);
                    }

                }
        */
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