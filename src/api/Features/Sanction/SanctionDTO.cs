using domain.Entity.Enum;

namespace api.Features.Sanctions;

public class SanctionDTO
{
    public string id_sanc { get; set; }
    public string id_membre { get; set; }
    public string? id_emp { get; set; }
    public string? email { get; set; }
    public DateTime date_emp { get; set; }
    public Raison_sanction[] raison { get; set; }
    public DateTime date_sanction { get; set; }
    public DateTime? date_fin_sanction { get; set; }
    public decimal? montant { get; set; }
    public bool? payement { get; set; }
    public bool active { get; set; } = true;
    public string? description { get; set; }
}
public class CreateSanctionRequest
{
    public string? email { get; set; }
    public string id_emp { get; set; }
    public Raison_sanction[] raison { get; set; }
    public DateTime? date_fin_sanction { get; set; }
    public decimal? montant { get; set; }
    public string? description { get; set; }

}