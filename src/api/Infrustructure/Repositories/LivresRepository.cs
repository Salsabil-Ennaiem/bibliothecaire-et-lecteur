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
        private readonly IFichierRepository _FichierRepository;

        public LivresRepository(BiblioDbContext dbContext, IFichierRepository FichierRepository)
        {
            _dbContext = dbContext;
            _FichierRepository = FichierRepository;
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
                            join i in _dbContext.Inventaires on l.id_livre equals i.id_liv
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
            if (string.IsNullOrEmpty(livreCreate.titre) ||
                string.IsNullOrEmpty(livreCreate.editeur) ||
                string.IsNullOrEmpty(livreCreate.date_edition) ||
                string.IsNullOrEmpty(livreCreate.cote_liv))
            {
                throw new Exception("Les 4 champs sont obligatoires : titre, éditeur, date édition, cote liv.");
            }



            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // Check for existing book by title and edition
                var existingLivre = await _dbContext.Livres
                    .FirstOrDefaultAsync(l => l.titre == livreCreate.titre && l.date_edition == livreCreate.date_edition);

                if (existingLivre != null)
                {
                    // Check if inventory cote_liv already exists
                    var inventaireExists = await _dbContext.Inventaires
                        .AnyAsync(i => i.cote_liv == livreCreate.cote_liv);

                    if (inventaireExists)
                    {
                        throw new Exception("Cote inventaire existe déjà.");
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

                    // Single call to save all changes
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return existingLivre.Adapt<LivreDTO>();
                }
                else
                {
                    // Create new book
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
                    };

                    if (livreCreate.couverture != null)
                    {
                        var couvEntity = livreCreate.couverture.Adapt<Fichier>();
                        var couv = await _FichierRepository.UploadImageAsync(couvEntity);
                        newLivre.couverture = couv;
                    }

                    await _dbContext.Livres.AddAsync(newLivre);

                    // Create new inventory linked to new book
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
                throw new Exception("Erreur lors de la création du livre et de l'inventaire: " + ex.Message, ex);
            }
        }
        public async Task<LivreDTO> UpdateAsync(string id, UpdateLivreDTO updatelivReq)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var liv = await GetByIdAsync(id);
                if (liv.statut != Statut_liv.emprunte)
                {
                    var inventaire = await GetInventaireByIdAsync(id);
                    var livre = inventaire.Livre;

                    if (updatelivReq.cote_liv is not null && RechercheCote(updatelivReq.cote_liv) is null)
                        inventaire.cote_liv = updatelivReq.cote_liv;
                    else throw new Exception(" Autre LIvre A ce placemnt ");

                    if (updatelivReq.etat is not null && updatelivReq.etat != etat_liv.neuf)
                        inventaire.etat = updatelivReq.etat;

                    if (updatelivReq.statut != Statut_liv.emprunte)
                        inventaire.statut = updatelivReq.statut;


                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return await GetByIdAsync(id);
                }
                else throw new Exception("Attender le livere doit etre non emprunte d'abord");
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
                var empExists = _dbContext.Emprunts.Any(e => e.Id_inv == id);
                if (!empExists)
                {
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
                            if (livre.couverture != null)
                            { await _FichierRepository.DeleteFileByIdAsync(livre.couverture); }
                            _dbContext.Livres.Remove(livre);
                        }
                    }
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    Console.WriteLine($"Livre with ID {id} deleted successfully.");
                }
                else { throw new Exception($"Error deleting Livre with ID {id}: le livre doit etre non emprunte par quelqu mem si perdu "); }
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
                return cot.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving Livre with cote {cote}: {ex.Message}", ex);
            }

        }
    }
}
