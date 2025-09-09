using Data;
using domain.Entity;
using domain.Interfaces;
using Microsoft.EntityFrameworkCore;

public class FichierRepository : IFichierRepository
{
    private readonly BiblioDbContext _context;

    public FichierRepository(BiblioDbContext context) => _context = context;

    public async Task<string> AddFileAsync(Fichier file)
    {
        var existing = await _context.Set<Fichier>()
            .FirstOrDefaultAsync(f => f.NomFichier == file.NomFichier && f.TailleFichier == file.TailleFichier);
        if (existing != null) return existing.IdFichier;

        if (string.IsNullOrWhiteSpace(file.IdFichier))
            file.IdFichier = Guid.NewGuid().ToString();

        _context.Set<Fichier>().Add(file);
        await _context.SaveChangesAsync();
        return file.IdFichier;
    }

    public async Task<IEnumerable<string>> AddFilesAsync(IEnumerable<Fichier> files)
    {
        var resultIds = new List<string>();
        foreach (var file in files)
        {
            var id = await AddFileAsync(file);
            resultIds.Add(id);
        }
        return resultIds;
    }

    public async Task<Fichier?> GetFileByIdAsync(string id)
    {
        return await _context.Set<Fichier>().FindAsync(id);
    }

    public async Task DeleteFileByIdAsync(string id)
    {
        var file = await _context.Set<Fichier>().FindAsync(id);
        if (file != null)
        {
            _context.Set<Fichier>().Remove(file);
            await _context.SaveChangesAsync();
        }
    }
}
