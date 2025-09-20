
using domain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Seeders;

public class UserSeeder
{
    public static async Task SeedUsersAsync(IServiceProvider serviceProvider)
    {
        try
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<Bibliothecaire>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Check if role exist
            var roleExists = await roleManager.RoleExistsAsync("Bibliothecaire");
            if (!roleExists)
            {
                await roleManager.CreateAsync(new IdentityRole("Bibliothecaire"));
                Console.WriteLine("✅ Created 'Bibliothecaire role");
            }
            else
            {
                Console.WriteLine("ℹ️ Role already exist");
            }

            var existingUsersCount = await userManager.Users.CountAsync();
            Console.WriteLine($"📊 Nombre d'utilisateurs existants: {existingUsersCount}");
            if (existingUsersCount >= 3)
            {
                Console.WriteLine("ℹ️ Users already exist in database");
                return;
            }

            var users = new List<(string email, string password, string nom, string prenom)>
            {

                ("salsabilnaim15@gmail.com", "Membre@123", "Membre", "User"),
                ("ennaiemsalsabil@gmail.com", "Biblio@123", "biblio", "isgs")


            };

            var createdUserIds = new List<string>();

            foreach (var (email, password, nom, prenom) in users)
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {

                    var newUser = new Bibliothecaire
                    {
                        // required fields :UserName, Email, normalized fields, and security-related data (PasswordHash) .
                        Email = email,
                        nom = nom,
                        prenom = prenom,
                        UserName = email,
                        NormalizedEmail = email
                    };
                    //hash passeword
                    var result = await userManager.CreateAsync(newUser, password);

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(newUser, "Bibliothecaire");
                        Console.WriteLine($"✅ Seeded user {email}");
                        createdUserIds.Add(newUser.Id);
                    }
                }
                else
                {
                    Console.WriteLine($"ℹ️ User {email} already exists.");
                    createdUserIds.Add(user.Id);
                }
            }

            if (createdUserIds.Count >= 2)
            {
                await DataSeeder.SeedAllDataAsync(serviceProvider);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error seeding users: {ex.Message}");
        }
    }
}
