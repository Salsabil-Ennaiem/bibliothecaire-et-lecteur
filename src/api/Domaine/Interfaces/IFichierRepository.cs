using domain.Entity;

namespace domain.Interfaces;
public interface IFichierRepository
{
    Task<Fichier> UploadF(IEnumerable<Fichier> files, string id);
    Task<string> UploadImageAsync(Fichier couverture);
   // Task<Fichier?> GetFileByIdAsync(string id);
    Task DeleteFileByIdAsync(string id);
    Task DeleteFileListAsync(string id);
    Task<(Stream? ContentStream, string ContentType, string FileName)?> GetFileByIdAsync(string id);
}
