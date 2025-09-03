using api.Features.Livre;
using Data;
using domain.Entity;
using domain.Entity.Enum;
using domain.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class LivresRepository : ILivresRepository
    {
        private readonly BiblioDbContext _dbContext;

        public LivresRepository(BiblioDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<LivreDTO>> GetAllLivresAsync()
        {
            return await (from i in _dbContext.Inventaires
                          join l in _dbContext.Livres on i.id_liv equals l.id_livre
                          select new LivreDTO
                          {
                              id_inv = i.id_inv,
                              date_edition = l.date_edition,
                              titre = l.titre,
                              auteur = l.auteur,
                              isbn = l.isbn,
                              editeur = l.editeur,
                              Description = l.Description,
                              Langue = l.Langue,
                              couverture = l.couverture,
                              cote_liv = i.cote_liv,
                              etat = i.etat,
                              statut = i.statut,
                              inventaire = i.inventaire
                          }).ToListAsync();
        }
        public async Task<LivreDTO> GetByIdAsync(string id)
        {
            try
            {
                var query = from l in _dbContext.Livres
                            join i in _dbContext.Inventaires
                                on l.id_livre equals i.id_liv
                            where i.id_inv == id
                            select new LivreDTO
                            {
                                id_inv = i.id_inv,
                                date_edition = l.date_edition,
                                titre = l.titre,
                                auteur = l.auteur,
                                isbn = l.isbn,
                                editeur = l.editeur,
                                Description = l.Description,
                                Langue = l.Langue,
                                couverture = l.couverture,
                                cote_liv = i.cote_liv,
                                etat = i.etat,
                                statut = i.statut,
                                inventaire = i.inventaire
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
        public async Task<LivreDTO> CreateAsync(CreateLivreRequest livreCreate)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                if (string.IsNullOrEmpty(livreCreate.titre) ||
                 string.IsNullOrEmpty(livreCreate.editeur) ||
                 string.IsNullOrEmpty(livreCreate.date_edition) ||
                 string.IsNullOrEmpty(livreCreate.cote_liv))
                {
                    throw new Exception("doit remplir les 4 champs sont obligatoire :titre , editeur , date edition , cote liv ");
                }

                if (RechercheCote(livreCreate.cote_liv) != null)
                {
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
                        return existingLivre.Adapt<LivreDTO>();

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
                       else{
                        throw new Exception("no yu should change that cote ");
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
        public async Task DeleteAsync(string id)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {

                var inventaire = await GetInventaireByIdAsync(id);
                var livreId = inventaire.id_liv;

                // Remove this physical book (Inventaire)
                _dbContext.Inventaires.Remove(inventaire);

                // Check if any other Inventaire references the same Livres
                bool hasOtherCopies = await _dbContext.Inventaires
                                 .AnyAsync(i => i.id_liv == livreId && i.id_inv != id);

                // If no other copies, remove the Livres itself
                if (!hasOtherCopies)
                {
                    var livre = await _dbContext.Livres.FindAsync(livreId);
                    if (livre != null)
                    {
                        _dbContext.Livres.Remove(livre);
                    }
                }
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                Console.WriteLine($"Livre with ID {id} deleted successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error deleting Livre with ID {id}: {ex.Message}", ex);
            }
        }
        private async Task<Inventaire?> GetInventaireByIdAsync(string inventaireId)
        {
            try
            {
                return await _dbContext.Inventaires
                    .Include(i => i.Livre)
                    .FirstOrDefaultAsync(i => i.id_inv == inventaireId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting Livre with ID {inventaireId}: {ex.Message}", ex);
            }
        }
        public string RechercheCote(string cote)
        {
            try
            {
                var cot = from l in _dbContext.Livres
                          join i in _dbContext.Inventaires
                              on l.id_livre equals i.id_liv
                          where i.cote_liv == cote &&
                          i.statut != Statut_liv.perdu
                          select i.id_inv;
                
                if (cot is null)
                {
                    throw new Exception($"{cot} with cote {cote} not found");
                }
                return  cot.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving Livre with cote {cote}: {ex.Message}", ex);
            }

        }
    }
}
