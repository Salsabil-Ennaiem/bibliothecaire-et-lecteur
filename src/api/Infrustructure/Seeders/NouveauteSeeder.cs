using System.Security.Cryptography;
using Data;
using domain.Entity;

namespace Infrastructure.Seeders
{
    public class NouveauteSeeder
    {
        private static readonly Random _random = new Random();

        // Generate random byte array of given length
        private static byte[] GenerateRandomBytes(int length)
        {
            var buffer = new byte[length];
            _random.NextBytes(buffer);
            return buffer;
        }

        // Compute SHA256 hash of byte array and return as base64 string
        private static string ComputeHash(byte[] data)
        {
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(data);
            return Convert.ToBase64String(hashBytes);
        }

        public static async Task SeedNouveautesAsync(BiblioDbContext dbContext)
        {
            var fichiers1 = new List<Fichier>();
            for (int i = 0; i < 3; i++)
            {
                int size = _random.Next(500_000, 3_000_000); // random size between ~0.5 MB to 3 MB
                var content = GenerateRandomBytes(size);
                var fichier = new Fichier
                {
                    IdFichier = Guid.NewGuid().ToString(),
                    NomFichier = $"fake_file_{i + 1}.pdf",
                    TypeFichier = "application/pdf",
                    TailleFichier = content.Length,
                    CheminFichier = null, // Assuming content stored in DB for seeding
                    DateCreation = DateTime.UtcNow,
                    ContenuFichier = content,
                    ContentHash = ComputeHash(content)
                };
                fichiers1.Add(fichier);
            }

            var couverture1Content = GenerateRandomBytes(100_000);
            var couverture1 = new Fichier
            {
                IdFichier = Guid.NewGuid().ToString(),
                NomFichier = "cover_image.jpg",
                TypeFichier = "image/jpeg",
                TailleFichier = couverture1Content.Length,
                CheminFichier = null,
                DateCreation = DateTime.UtcNow,
                ContenuFichier = couverture1Content,
                ContentHash = ComputeHash(couverture1Content)
            };

            var nouveaute1 = new Nouveaute
            {
                id_nouv = Guid.NewGuid().ToString(),
                titre = "Nouvelle Collection Informatique",
                description = "Découvrez notre nouvelle collection de livres d'informatique pour 2024. Plus de 50 nouveaux titres disponibles !",
                date_publication = DateTime.UtcNow.AddDays(-7),
                couverture = couverture1.IdFichier,
                Couvertures = couverture1,
                Fichiers = fichiers1
            };

            foreach (var f in fichiers1)
            {
                f.NouveauteId = nouveaute1.id_nouv;
                f.ficherNouv = nouveaute1;
            }
            couverture1.couvertureNouv = nouveaute1;

            var couverture2Content = GenerateRandomBytes(100_000);
            var couverture2 = new Fichier
            {
                IdFichier = Guid.NewGuid().ToString(),
                NomFichier = "cover_image2.jpg",
                TypeFichier = "image/jpeg",
                TailleFichier = couverture2Content.Length,
                CheminFichier = null,
                DateCreation = DateTime.UtcNow,
                ContenuFichier = couverture2Content,
                ContentHash = ComputeHash(couverture2Content)
            };

            var nouveaute2 = new Nouveaute
            {
                id_nouv = Guid.NewGuid().ToString(),
                titre = "Horaires d'été 2024",
                description = "Nouveaux horaires d'ouverture pour la période estivale. La bibliothèque sera ouverte de 8h à 16h du lundi au vendredi.",
                date_publication = DateTime.UtcNow.AddDays(-3),
                couverture = couverture2.IdFichier,
                Couvertures = couverture2,
                Fichiers = new List<Fichier>() // No additional files here
            };

            couverture2.couvertureNouv = nouveaute2;

            await dbContext.Fichiers.AddRangeAsync(fichiers1);
            await dbContext.Fichiers.AddAsync(couverture1);
            await dbContext.Fichiers.AddAsync(couverture2);

            await dbContext.Nouveautes.AddAsync(nouveaute1);
            await dbContext.Nouveautes.AddAsync(nouveaute2);

            await dbContext.SaveChangesAsync();

            Console.WriteLine("✅ Seeded nouveautes with random fichiers and covers");
        }
    }
}
