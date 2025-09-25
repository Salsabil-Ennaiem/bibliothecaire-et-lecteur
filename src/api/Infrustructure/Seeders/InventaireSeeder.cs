using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using domain.Entity;
using domain.Entity.Enum;

namespace Infrastructure.Seeders
{
    public class InventaireSeeder
    {
        public static async Task<List<Inventaire>> SeedInventairesAsync(BiblioDbContext dbContext, List<Livres> livres)
        {
            var inventaires = new List<Inventaire>
            {
                new Inventaire { id_inv = Guid.NewGuid().ToString(), id_liv = livres[0].id_livre, cote_liv = "PROG-001", etat = etat_liv.neuf, statut = Statut_liv.disponible, inventaire = "INV-2024-001" },
                new Inventaire { id_inv = Guid.NewGuid().ToString(), id_liv = livres[1].id_livre, cote_liv = "DATA-001", etat = etat_liv.moyen, statut = Statut_liv.emprunte, inventaire = "INV-2024-002" },
                new Inventaire { id_inv = Guid.NewGuid().ToString(), id_liv = livres[2].id_livre, cote_liv = "ARCH-001", etat = etat_liv.moyen, statut = Statut_liv.emprunte, inventaire = "INV-2024-003" },
                new Inventaire { id_inv = Guid.NewGuid().ToString(), id_liv = livres[3].id_livre, cote_liv = "JS-001", etat = etat_liv.mauvais, statut = Statut_liv.perdu, inventaire = "INV-2024-004" },
                new Inventaire { id_inv = Guid.NewGuid().ToString(), id_liv = livres[4].id_livre, cote_liv = "PAT-001", etat = etat_liv.moyen, statut = Statut_liv.disponible, inventaire = "INV-2024-005" },
                new Inventaire { id_inv = Guid.NewGuid().ToString(), id_liv = livres[5].id_livre, cote_liv = "DEV-001", etat = etat_liv.neuf, statut = Statut_liv.disponible, inventaire = "INV-2024-006" },
                new Inventaire { id_inv = Guid.NewGuid().ToString(), id_liv = livres[6].id_livre, cote_liv = "REA-001", etat = etat_liv.mauvais, statut = Statut_liv.emprunte, inventaire = "INV-2024-007" },
                new Inventaire { id_inv = Guid.NewGuid().ToString(), id_liv = livres[7].id_livre, cote_liv = "PYT-001", etat = etat_liv.neuf, statut = Statut_liv.disponible, inventaire = "INV-2024-008" },
                new Inventaire { id_inv = Guid.NewGuid().ToString(), id_liv = livres[8].id_livre, cote_liv = "KUB-001", etat = etat_liv.moyen, statut = Statut_liv.emprunte, inventaire = "INV-2024-009" },
                new Inventaire { id_inv = Guid.NewGuid().ToString(), id_liv = livres[9].id_livre, cote_liv = "ML-001", etat = etat_liv.mauvais, statut = Statut_liv.disponible, inventaire = "INV-2024-010" }
            };

            await dbContext.Inventaires.AddRangeAsync(inventaires);
            await dbContext.SaveChangesAsync();
            Console.WriteLine($"✅ Seeded {inventaires.Count} inventaires");

            return inventaires;
        }
    }
}
