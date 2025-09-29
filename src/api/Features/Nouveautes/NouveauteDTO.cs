using api.Common;

namespace api.Features.Nouveautes
{
    public class NouveauteDTO
    {
        public string? id_nouv { get; set; }
        public string? titre { get; set; }
        public string? fichier { get; set; }
        public string? description { get; set; }
        public DateTime date_publication { get; set; } = DateTime.UtcNow;
        public string? couverture { get; set; }

           public FichierDto? CouvertureFile { get; set; }

    // Collection of related fichiers (files)
    public List<FichierDto>? Fichiers { get; set; }

    }
    public class NouveauteGetALL
    {
        public string? id_nouv { get; set; }
        public DateTime date_publication { get; set; } = DateTime.UtcNow;
        public string? couverture { get; set; }
        public string? titre { get; set; }
        public FichierDto? CouvertureFile { get; set; }

    }

    public class CreateNouveauteRequestWithFiles
    {
        public string titre { get; set; }
        public string? description { get; set; }
        public FichierDto? Couv { get; set; }
        public IEnumerable<FichierDto>? File { get; set; }
    }

}