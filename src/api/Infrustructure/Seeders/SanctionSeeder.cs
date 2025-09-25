using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using domain.Entity;
using domain.Entity.Enum;

namespace Infrastructure.Seeders
{
    public class SanctionSeeder
    {
        public static async Task SeedSanctionsAsync(BiblioDbContext dbContext, List<Emprunts> emprunts, List<Membre> membres)
        {
            var sanctions = new List<Sanction>
            {
                new Sanction { id_sanc = Guid.NewGuid().ToString(), id_membre = membres[0].id_membre, id_emp = emprunts[0].id_emp, raison = [Raison_sanction.retard], date_sanction = DateTime.UtcNow.AddDays(-700), date_fin_sanction = DateTime.UtcNow.AddDays(-680), montant = 15, payement = false, active = false, description = "Retard il y a 2 ans" },
                new Sanction { id_sanc = Guid.NewGuid().ToString(), id_membre = membres[1].id_membre, id_emp = emprunts[1].id_emp, raison = [Raison_sanction.perte], date_sanction = DateTime.UtcNow.AddDays(-650), date_fin_sanction = DateTime.UtcNow.AddDays(-620), montant = 45, payement = false, active = false, description = "Perte il y a 22 mois" },
                new Sanction { id_sanc = Guid.NewGuid().ToString(), id_membre = membres[2].id_membre, id_emp = emprunts[2].id_emp, raison = [Raison_sanction.retard], date_sanction = DateTime.UtcNow.AddDays(-600), date_fin_sanction = DateTime.UtcNow.AddDays(-590), montant = 10, payement = true, active = true, description = "Retard recent payé" },
                new Sanction { id_sanc = Guid.NewGuid().ToString(), id_membre = membres[3].id_membre, id_emp = emprunts[3].id_emp, raison = [Raison_sanction.perte], date_sanction = DateTime.UtcNow.AddDays(-550), date_fin_sanction = DateTime.UtcNow.AddDays(-520), montant = 50, payement = false, active = true, description = "Perte confirmée" },
                new Sanction { id_sanc = Guid.NewGuid().ToString(), id_membre = membres[4].id_membre, id_emp = emprunts[4].id_emp, raison = [Raison_sanction.retard], date_sanction = DateTime.UtcNow.AddDays(-400), date_fin_sanction = DateTime.UtcNow.AddDays(-380), montant = 20, payement = false, active = false, description = "Retard régulier" },
                new Sanction { id_sanc = Guid.NewGuid().ToString(), id_membre = membres[5].id_membre, id_emp = emprunts[5].id_emp, raison = [Raison_sanction.perte], date_sanction = DateTime.UtcNow.AddDays(-350), date_fin_sanction = DateTime.UtcNow.AddDays(-320), montant = 35, payement = false, active = false, description = "Perte récente" },
                new Sanction { id_sanc = Guid.NewGuid().ToString(), id_membre = membres[6].id_membre, id_emp = emprunts[6].id_emp, raison = [Raison_sanction.retard], date_sanction = DateTime.UtcNow.AddDays(-300), date_fin_sanction = DateTime.UtcNow.AddDays(-280), montant = 18, payement = true, active = true, description = "Retard payé" },
                new Sanction { id_sanc = Guid.NewGuid().ToString(), id_membre = membres[7].id_membre, id_emp = emprunts[7].id_emp, raison = [Raison_sanction.perte], date_sanction = DateTime.UtcNow.AddDays(-240), date_fin_sanction = DateTime.UtcNow.AddDays(-210), montant = 40, payement = false, active = true, description = "Perte confirmée" },
                new Sanction { id_sanc = Guid.NewGuid().ToString(), id_membre = membres[8].id_membre, id_emp = emprunts[8].id_emp, raison = [Raison_sanction.retard], date_sanction = DateTime.UtcNow.AddDays(-180), date_fin_sanction = DateTime.UtcNow.AddDays(-150), montant = 25, payement = false, active = false, description = "Retard divers" },
                new Sanction { id_sanc = Guid.NewGuid().ToString(), id_membre = membres[9].id_membre, id_emp = emprunts[9].id_emp, raison = [Raison_sanction.perte], date_sanction = DateTime.UtcNow.AddDays(-120), date_fin_sanction = DateTime.UtcNow.AddDays(-90), montant = 50, payement = false, active = true, description = "Perte ancienne" }
            };

            await dbContext.Sanctions.AddRangeAsync(sanctions);
            await dbContext.SaveChangesAsync();
            Console.WriteLine($"✅ Seeded {sanctions.Count} sanctions over 2 years");
        }
    }
}
