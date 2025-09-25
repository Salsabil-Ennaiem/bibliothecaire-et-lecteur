using System;
using System.Threading.Tasks;
using Data;
using domain.Entity;

namespace Infrastructure.Seeders
{
    public class ParametreSeeder
    {
        public static async Task<(Parametre ancien, Parametre nouveau)> SeedParametresAsync(BiblioDbContext dbContext)
        {
            var ancienParametre = new Parametre
            {
                id_param = Guid.NewGuid().ToString(),
                Delais_Emprunt_Etudiant = 7,
                Delais_Emprunt_Enseignant = 10,
                Delais_Emprunt_Autre = 5,
                Modele_Email_Retard = "Rappel: Votre livre [TITRE] est en retard depuis le [DATE]",
                date_modification = DateTime.UtcNow.AddDays(-365) // 1 year ago
            };

            var nouveauParametre = new Parametre
            {
                id_param = Guid.NewGuid().ToString(),
                Delais_Emprunt_Etudiant = 14,
                Delais_Emprunt_Enseignant = 20,
                Delais_Emprunt_Autre = 10,
                Modele_Email_Retard = "URGENT: Votre livre [TITRE] est en retard depuis le [DATE]. Merci de le retourner rapidement.",
                date_modification = DateTime.UtcNow.AddDays(-345) // 20 days later
            };

            await dbContext.Parametres.AddRangeAsync(new[] { ancienParametre, nouveauParametre });
            await dbContext.SaveChangesAsync();
            Console.WriteLine("✅ Seeded 2 parametres spaced 20 days apart over last 2 years");

            return (ancienParametre, nouveauParametre);
        }
    }
}
