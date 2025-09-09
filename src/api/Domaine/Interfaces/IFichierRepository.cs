using domain.Entity;

namespace domain.Interfaces;
public interface IFichierRepository
{
    Task<string> AddFileAsync(Fichier file);
    Task<IEnumerable<string>> AddFilesAsync(IEnumerable<Fichier> files);
    Task<Fichier?> GetFileByIdAsync(string id);
    Task DeleteFileByIdAsync(string id);
}
