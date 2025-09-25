using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using domain.Entity;

namespace Infrastructure.Seeders
{
    public class LivreSeeder
    {
        public static async Task<List<Livres>> SeedLivresAsync(BiblioDbContext dbContext)
        {
            var livres = new List<Livres>
            {
                new Livres { id_livre = Guid.NewGuid().ToString(), date_edition = "2023", titre = "Programmation C#", auteur = "Jean Martin", isbn = "978-2-1234-5678-9", editeur = "Editions Tech", Description = "Guide complet C#", Langue = "Français" },
                new Livres { id_livre = Guid.NewGuid().ToString(), date_edition = "2022", titre = "Base de Données", auteur = "Marie Durand", isbn = "978-2-9876-5432-1", editeur = "Editions Data", Description = "Bases relationnelles", Langue = "Français" },
                new Livres { id_livre = Guid.NewGuid().ToString(), date_edition = "2024", titre = "Architecture Web", auteur = "Pierre Blanc", isbn = "978-2-5555-7777-3", editeur = "Editions Web", Description = "Architecture moderne", Langue = "Français" },
                new Livres { id_livre = Guid.NewGuid().ToString(), date_edition = "2023", titre = "JavaScript Avancé", auteur = "Sophie Bernard", isbn = "978-2-3333-4444-5", editeur = "Editions JS", Description = "JavaScript pour experts", Langue = "Français" },
                new Livres { id_livre = Guid.NewGuid().ToString(), date_edition = "2022", titre = "Design Patterns", auteur = "Robert C. Martin", isbn = "978-2-6666-7777-8", editeur = "Editions Software", Description = "Patterns avancées", Langue = "Anglais" },
                new Livres { id_livre = Guid.NewGuid().ToString(), date_edition = "2024", titre = "DevOps Guide", auteur = "Patrick Dubois", isbn = "978-2-9999-8888-7", editeur = "Editions Ops", Description = "Best practices DevOps", Langue = "Français" },
                new Livres { id_livre = Guid.NewGuid().ToString(), date_edition = "2023", titre = "React Applications", auteur = "Lisa Wong", isbn = "978-2-2222-3333-4", editeur = "Editions Web", Description = "React avancé", Langue = "Anglais" },
                new Livres { id_livre = Guid.NewGuid().ToString(), date_edition = "2023", titre = "Python Data Science", auteur = "Thomas Lee", isbn = "978-2-4444-5555-6", editeur = "Editions Data", Description = "Data science Python", Langue = "Français" },
                new Livres { id_livre = Guid.NewGuid().ToString(), date_edition = "2024", titre = "Kubernetes Essentials", auteur = "Nina Patel", isbn = "978-2-7777-8888-9", editeur = "Editions Cloud", Description = "Containers orchestration", Langue = "Anglais" },
                new Livres { id_livre = Guid.NewGuid().ToString(), date_edition = "2022", titre = "Machine Learning", auteur = "Alan Turing", isbn = "978-2-1111-2222-3", editeur = "Editions AI", Description = "ML fundamentals", Langue = "Français" }
            };

            await dbContext.Livres.AddRangeAsync(livres);
            await dbContext.SaveChangesAsync();
            Console.WriteLine($"✅ Seeded {livres.Count} books");

            return livres;
        }
    }
}
