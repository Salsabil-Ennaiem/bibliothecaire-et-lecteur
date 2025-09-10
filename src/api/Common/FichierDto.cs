namespace api.Common;
public class FichierDto
{
    public required string IdFichier { get; set; }
    public string? NomFichier { get; set; }
    public string? CheminFichier { get; set; }
    public string? TypeFichier { get; set; }
    public byte[]? ContenuFichier { get; set; }
    public required string ContentHash { get; set; }     
    public long TailleFichier { get; set; }
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;
    public string? NouveauteId { get; set; }
}