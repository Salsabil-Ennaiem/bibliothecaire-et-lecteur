using domain.Entity.Enum;

namespace api.Features.Membres;

public class MembreDto
{
    public string id_membre { get; set; }
    public TypeMemb TypeMembre { get; set; }
    public string? nom { get; set; }
    public string? prenom { get; set; }
    public required string email { get; set; }
    public string? telephone { get; set; }
    public required string cin_ou_passeport { get; set; }
    public DateTime date_inscription { get; set; } 
    public StatutMemb Statut { get; set; } 
}
public class UpdateMembreDto
{
    public TypeMemb TypeMembre { get; set; }
    public string? nom { get; set; }
    public string? prenom { get; set; }
    public  string email { get; set; }
    public string? telephone { get; set; }
}
