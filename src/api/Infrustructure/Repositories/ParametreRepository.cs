using api.Features.Parametre;
using Data;
using domain.Entity;
using domain.Entity.Enum;
using domain.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositries
{
    public class ParametreRepository : IParametreRepository
    {
        private readonly BiblioDbContext _context;

        public ParametreRepository(BiblioDbContext context)
        {
            _context = context;
        }

        public async Task<ParametreDTO> GetParam()
        {
            try
            {
                var parametre = await _context.Parametres
                    .OrderByDescending(p => p.date_modification)
                    .FirstOrDefaultAsync();
                return parametre.Adapt<ParametreDTO>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving Parametre : {ex.Message}", ex);
            }
        }
        public async Task<ParametreDTO> Updatepram(UpdateParametreDTO entity)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var existingParam = await GetParam();
                if (!AreParametersEqual(entity, existingParam))
                {
                    var paramEntity = entity.Adapt<Parametre>();
                    paramEntity.id_param = Guid.NewGuid().ToString();
                    paramEntity.date_modification = DateTime.UtcNow;
                    if (paramEntity.Delais_Emprunt_Autre == null)
                    {
                        paramEntity.Delais_Emprunt_Autre = existingParam.Delais_Emprunt_Autre;
                    }
                    if (paramEntity.Delais_Emprunt_Enseignant == null)
                    {
                        paramEntity.Delais_Emprunt_Enseignant = existingParam.Delais_Emprunt_Enseignant;
                    }
                    if (paramEntity.Delais_Emprunt_Etudiant == null)
                    {
                        paramEntity.Delais_Emprunt_Etudiant = existingParam.Delais_Emprunt_Etudiant;
                    }
                    if (paramEntity.Modele_Email_Retard == null)
                    { paramEntity.Modele_Email_Retard = existingParam.Modele_Email_Retard; }
                    await _context.Parametres.AddAsync(paramEntity);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                return entity.Adapt<ParametreDTO>();

            }

            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error updating Parametre : {ex.Message}", ex);
            }

        }
        public async Task<int> GetDelais(TypeMemb type)
        {
            var param = await GetParam();
            if (param == null)
                throw new Exception("Paramètre non trouvé");
            if (type == TypeMemb.Autre)
                return param.Delais_Emprunt_Autre;
            else if (type == TypeMemb.Etudiant)
                return param.Delais_Emprunt_Etudiant;
            else
                return param.Delais_Emprunt_Enseignant;

        }
        private bool AreParametersEqual(UpdateParametreDTO p1, ParametreDTO p2)
        {
            if (p1 == null || p2 == null) return false;

            return p1.Delais_Emprunt_Etudiant == p2.Delais_Emprunt_Etudiant &&
                   p1.Delais_Emprunt_Enseignant == p2.Delais_Emprunt_Enseignant &&
                   p1.Delais_Emprunt_Autre == p2.Delais_Emprunt_Autre &&
                   p1.Modele_Email_Retard == p2.Modele_Email_Retard;
        }
    }

}