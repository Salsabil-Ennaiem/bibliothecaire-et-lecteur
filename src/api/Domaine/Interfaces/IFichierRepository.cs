using api.Common;
using domain.Entity;

namespace domain.Interfaces;
public interface IFichierRepository
{
    Task<Fichier> UploadF(IEnumerable<Fichier> files, string id);
    Task<string> UploadImageAsync(Fichier couverture);
    Task<FichierDto?> GetFullFileInfoAsync(string id);
    Task<IEnumerable<FichierDto>> GetByNouveauteIdAsync(string nouveauteId);
//    Task<(Stream? ContentStream, string ContentType, string FileName)?> GetFileByIdAsync(string id);
        Task DeleteFileByIdAsync(string id);
    Task DeleteFileListAsync(string id);

}
