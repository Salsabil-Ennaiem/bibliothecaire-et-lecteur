using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using domain.Entity;
using domain.Entity.Enum;

namespace Infrastructure.Seeders
{
    public class MembreSeeder
    {
        public static async Task<List<Membre>> SeedMembresAsync(BiblioDbContext dbContext)
        {
            var membres = new List<Membre>
            {
                new Membre { id_membre = Guid.NewGuid().ToString(), TypeMembre = TypeMemb.Etudiant, nom="Benali", prenom="Ahmed", email="ahmed.benali@email.com", telephone="+21620123456", cin_ou_passeport="12345678", date_inscription = DateTime.UtcNow.AddMonths(-6), Statut = StatutMemb.actif },
                new Membre { id_membre = Guid.NewGuid().ToString(), TypeMembre = TypeMemb.Enseignant, nom="Trabelsi", prenom="Fatma", email="fatma.trabelsi@email.com", telephone="+21625987654", cin_ou_passeport="87654321", date_inscription = DateTime.UtcNow.AddMonths(-3), Statut = StatutMemb.actif },
                new Membre { id_membre = Guid.NewGuid().ToString(), TypeMembre = TypeMemb.Autre, nom="Khelifi", prenom="Mohamed", email="mohamed.khelifi@email.com", telephone="+21622555777", cin_ou_passeport="11223344", date_inscription = DateTime.UtcNow.AddMonths(-1), Statut = StatutMemb.sanctionne },
                new Membre { id_membre = Guid.NewGuid().ToString(), TypeMembre = TypeMemb.Etudiant, nom="Sara", prenom="Omar", email="sara.omar@email.com", telephone="+21620333444", cin_ou_passeport="22334455", date_inscription = DateTime.UtcNow.AddMonths(-5), Statut = StatutMemb.actif },
                new Membre { id_membre = Guid.NewGuid().ToString(), TypeMembre = TypeMemb.Enseignant, nom="Fathi", prenom="Youssef", email="fathi.youssef@email.com", telephone="+21620777888", cin_ou_passeport="99887766", date_inscription = DateTime.UtcNow.AddMonths(-2), Statut = StatutMemb.actif },
                new Membre { id_membre = Guid.NewGuid().ToString(), TypeMembre = TypeMemb.Etudiant, nom="Amina", prenom="Lina", email="amin.lina@email.com", telephone="+21620112233", cin_ou_passeport="44556677", date_inscription = DateTime.UtcNow.AddMonths(-4), Statut = StatutMemb.actif },
                new Membre { id_membre = Guid.NewGuid().ToString(), TypeMembre = TypeMemb.Autre, nom="Hamdi", prenom="Issam", email="hamdi.issam@email.com", telephone="+21620445566", cin_ou_passeport="66554433", date_inscription = DateTime.UtcNow.AddMonths(-2), Statut = StatutMemb.sanctionne },
                new Membre { id_membre = Guid.NewGuid().ToString(), TypeMembre = TypeMemb.Enseignant, nom="Salma", prenom="Nour", email="salma.nour@email.com", telephone="+21620998877", cin_ou_passeport="77889900", date_inscription = DateTime.UtcNow.AddMonths(-7), Statut = StatutMemb.actif },
                new Membre { id_membre = Guid.NewGuid().ToString(), TypeMembre = TypeMemb.Autre, nom="Lotfi", prenom="Karim", email="lotfi.karim@email.com", telephone="+21620111222", cin_ou_passeport="44557788", date_inscription = DateTime.UtcNow.AddMonths(-1), Statut = StatutMemb.actif },
                new Membre { id_membre = Guid.NewGuid().ToString(), TypeMembre = TypeMemb.Etudiant, nom="Nadia", prenom="Maha", email="nadia.maha@email.com", telephone="+21620334455", cin_ou_passeport="11224455", date_inscription = DateTime.UtcNow.AddMonths(-3), Statut = StatutMemb.actif }
            };

            await dbContext.Membres.AddRangeAsync(membres);
            await dbContext.SaveChangesAsync();
            Console.WriteLine($"✅ Seeded {membres.Count} membres");

            return membres;
        }
    }
}
