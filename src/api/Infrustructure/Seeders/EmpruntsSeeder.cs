using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using domain.Entity;
using domain.Entity.Enum;

namespace Infrastructure.Seeders
{
    public class EmpruntsSeeder
    {
        public static async Task<List<Emprunts>> SeedEmpruntsAsync(BiblioDbContext dbContext, List<Membre> membres, List<Inventaire> inventaires)
        {
            var emprunts = new List<Emprunts>
            {
                new Emprunts { id_emp = Guid.NewGuid().ToString(), id_membre = membres[0].id_membre, Id_inv = inventaires[0].id_inv, date_emp = DateTime.UtcNow.AddDays(-730), date_retour_prevu = DateTime.UtcNow.AddDays(-720), date_effectif = DateTime.UtcNow.AddDays(-719), Statut_emp = Statut_emp.retourne, note = "Retour à temps 2 ans" },
                new Emprunts { id_emp = Guid.NewGuid().ToString(), id_membre = membres[1].id_membre, Id_inv = inventaires[1].id_inv, date_emp = DateTime.UtcNow.AddDays(-600), date_retour_prevu = DateTime.UtcNow.AddDays(-590), date_effectif = null, Statut_emp = Statut_emp.en_cours, note = "Emprunt en cours il y a 20 mois" },
                new Emprunts { id_emp = Guid.NewGuid().ToString(), id_membre = membres[2].id_membre, Id_inv = inventaires[2].id_inv, date_emp = DateTime.UtcNow.AddDays(-540), date_retour_prevu = DateTime.UtcNow.AddDays(-530), date_effectif = DateTime.UtcNow.AddDays(-528), Statut_emp = Statut_emp.retourne, note = "Retour tardif il y a 18 mois" },
                new Emprunts { id_emp = Guid.NewGuid().ToString(), id_membre = membres[3].id_membre, Id_inv = inventaires[3].id_inv, date_emp = DateTime.UtcNow.AddDays(-480), date_retour_prevu = DateTime.UtcNow.AddDays(-470), date_effectif = null, Statut_emp = Statut_emp.perdu, note = "Livre perdu il y a 16 mois" },
                new Emprunts { id_emp = Guid.NewGuid().ToString(), id_membre = membres[4].id_membre, Id_inv = inventaires[4].id_inv, date_emp = DateTime.UtcNow.AddDays(-420), date_retour_prevu = DateTime.UtcNow.AddDays(-410), date_effectif = DateTime.UtcNow.AddDays(-408), Statut_emp = Statut_emp.retourne, note = "Retour normal il y a 14 mois" },
                new Emprunts { id_emp = Guid.NewGuid().ToString(), id_membre = membres[5].id_membre, Id_inv = inventaires[5].id_inv, date_emp = DateTime.UtcNow.AddDays(-360), date_retour_prevu = DateTime.UtcNow.AddDays(-350), date_effectif = null, Statut_emp = Statut_emp.en_cours, note = "Emprunt récent" },
                new Emprunts { id_emp = Guid.NewGuid().ToString(), id_membre = membres[6].id_membre, Id_inv = inventaires[6].id_inv, date_emp = DateTime.UtcNow.AddDays(-300), date_retour_prevu = DateTime.UtcNow.AddDays(-290), date_effectif = null, Statut_emp = Statut_emp.retourne, note = "Retour à temps" },
                new Emprunts { id_emp = Guid.NewGuid().ToString(), id_membre = membres[7].id_membre, Id_inv = inventaires[7].id_inv, date_emp = DateTime.UtcNow.AddDays(-240), date_retour_prevu = DateTime.UtcNow.AddDays(-230), date_effectif = DateTime.UtcNow.AddDays(-228), Statut_emp = Statut_emp.retourne, note = "Retour dans les temps" },
                new Emprunts { id_emp = Guid.NewGuid().ToString(), id_membre = membres[8].id_membre, Id_inv = inventaires[8].id_inv, date_emp = DateTime.UtcNow.AddDays(-180), date_retour_prevu = DateTime.UtcNow.AddDays(-170), date_effectif = null, Statut_emp = Statut_emp.en_cours, note = "En cours" },
                new Emprunts { id_emp = Guid.NewGuid().ToString(), id_membre = membres[9].id_membre, Id_inv = inventaires[9].id_inv, date_emp = DateTime.UtcNow.AddDays(-120), date_retour_prevu = DateTime.UtcNow.AddDays(-110), date_effectif = DateTime.UtcNow.AddDays(-108), Statut_emp = Statut_emp.retourne, note = "Emprunt récent retourné" }
            };

            await dbContext.Emprunts.AddRangeAsync(emprunts);
            await dbContext.SaveChangesAsync();
            Console.WriteLine($"✅ Seeded {emprunts.Count} emprunts over 2 years");

            return emprunts;
        }
    }
}
