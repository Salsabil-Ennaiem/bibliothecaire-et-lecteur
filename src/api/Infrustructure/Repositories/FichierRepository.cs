using System.Security.Cryptography;
using api.Common;
using Data;
using domain.Entity;
using domain.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;

public class FichierRepository : IFichierRepository
{
    private readonly BiblioDbContext _context;

    public FichierRepository(BiblioDbContext context) => _context = context;

    public async Task<Fichier> UploadF(IEnumerable<Fichier> files, string id)
    {

        if (files == null || !files.Any())
            throw new Exception("File collection is empty");

        //Prevents exceptions if the folder is missing.
        var uploadsFolder = Path.Combine("uploads");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        //if one file no need to take all that time under forech           
        if (files.Count() == 1)
        {
            var f = files.Last();
            await UploadImageAsync(f);
            return f;
        }

        foreach (var fichier in files)
        {
            using var ms = new MemoryStream();
            if (fichier.ContenuFichier != null && fichier.ContenuFichier.Length > 0)
            {
                await ms.WriteAsync(fichier.ContenuFichier);
            }
            byte[] fileBytes = ms.ToArray();

            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(fileBytes);
            string contentHash = Convert.ToBase64String(hashBytes);
            fichier.NouveauteId = id;
            var existingFile = await ExistingFichierAsync(contentHash);
            if (existingFile == null)
            {
                if (fileBytes.Length > 1_000_000) // example threshold 1MB
                {
                    var savePath = Path.Combine("uploads", fichier.IdFichier + Path.GetExtension(fichier.NomFichier ?? ""));
                    await File.WriteAllBytesAsync(savePath, fileBytes);
                    fichier.CheminFichier = savePath;
                    fichier.ContenuFichier = null;
                }
                else
                {
                    fichier.ContenuFichier = fileBytes;
                    fichier.CheminFichier = null;
                }
                fichier.ContentHash = contentHash;
                _context.Set<Fichier>().Add(fichier);
                await _context.SaveChangesAsync();
            }
            else { fichier.IdFichier = existingFile.IdFichier; }
        }

        return files.Last();
    }
    public async Task<string> UploadImageAsync(Fichier couverture)
    {
        if (couverture.ContenuFichier == null || couverture.ContenuFichier.Length == 0)
            throw new ArgumentException("File content is empty");

        // Calculer le hash SHA256 du contenu
        using var sha256 = SHA256.Create();
        byte[] hashBytes = sha256.ComputeHash(couverture.ContenuFichier!);
        string contentHash = Convert.ToBase64String(hashBytes);

        //Prevents exceptions if the folder is missing.
        var uploadsFolder = Path.Combine("uploads");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        // Vérifier si le couverture existe déjà
        var existingFile = await ExistingFichierAsync(contentHash);
        if (existingFile != null)
        {
            return existingFile.IdFichier;
        }

        // Nouveau couverture, enregistrer
        couverture.ContentHash = contentHash;
        couverture.IdFichier = Guid.NewGuid().ToString();

        if (couverture.TailleFichier > 1_000_000) // seuil 1 Mo exemple
        {
            var savePath = Path.Combine("uploads", couverture.IdFichier + Path.GetExtension(couverture.NomFichier ?? ""));
            await File.WriteAllBytesAsync(savePath, couverture.ContenuFichier!);
            couverture.CheminFichier = savePath;
            couverture.ContenuFichier = null;
        }

        _context.Fichiers.Add(couverture);
        await _context.SaveChangesAsync();

        return couverture.IdFichier;
    }
  /*  public async Task<(Stream ContentStream, string ContentType, string FileName)?> GetFileByIdAsync(string id)
    {
        var fichier = await _context.Set<Fichier>().FindAsync(id);
        if (fichier == null)
            throw new KeyNotFoundException($"File with id '{id}' not found.");

        if (!string.IsNullOrEmpty(fichier.CheminFichier) && File.Exists(fichier.CheminFichier))
        {
            try
            {
                // Stream file from file system.
                // Note: Caller is responsible for disposing the returned Stream to avoid file handle leaks.
                var stream = File.OpenRead(fichier.CheminFichier);
                return (stream, fichier.TypeFichier ?? "application/octet-stream", fichier.NomFichier ?? "file");
            }
            catch (IOException ex)
            {
                throw new IOException($"Error opening file at path '{fichier.CheminFichier}'.", ex);
            }
        }
        else if (fichier.ContenuFichier != null && fichier.ContenuFichier.Length > 0)
        {
            // Return a memory stream from DB blob content
            var ms = new MemoryStream(fichier.ContenuFichier);
            return (ms, fichier.TypeFichier ?? "application/octet-stream", fichier.NomFichier ?? "file");
        }
        else
        {
            throw new InvalidOperationException($"File content for id '{id}' not found in file path or database.");
        }
    }
  */
    public async Task<FichierDto?> GetFullFileInfoAsync(string fileId)
    {
        var fichier = await _context.Fichiers.FindAsync(fileId);
        if (fichier == null)
            return null;

        var dto = fichier.Adapt<FichierDto>();

        if (!string.IsNullOrEmpty(fichier.CheminFichier) && File.Exists(fichier.CheminFichier))
        {
            try
            {
                dto.ContenuFichier = await File.ReadAllBytesAsync(fichier.CheminFichier);
            }
            catch (IOException ex)
            {
                throw new IOException($"Error reading file content at path '{fichier.CheminFichier}'.", ex);
            }
        }
        else if (fichier.ContenuFichier != null && fichier.ContenuFichier.Length > 0)
        {
            dto.ContenuFichier = fichier.ContenuFichier;
        }
        else
        {
            dto.ContenuFichier = null;
        }

        return dto;
    }
    public async Task<IEnumerable<FichierDto>> GetByNouveauteIdAsync(string nouveauteId)
    {
        var fichiers = await _context.Fichiers
            .Where(f => f.NouveauteId == nouveauteId)
            .ToListAsync();

        if (!fichiers.Any())
            return Enumerable.Empty<FichierDto>();

        var dtos = fichiers.Adapt<List<FichierDto>>();

        var fullDtos = await Task.WhenAll(
            dtos.Select(async dto => await GetFullFileInfoAsync(dto.IdFichier) ?? dto)
        );

        return fullDtos;
    }
    public async Task DeleteFileByIdAsync(string fileId)
    {
        var file = await _context.Set<Fichier>().FindAsync(fileId);
        if (file == null)
            throw new Exception("File not found.");

        bool isUsedElsewhere = await IsFileUsedElsewhereAsync(fileId);
        if (file.NouveauteId == null && !isUsedElsewhere)
        {
            _context.Set<Fichier>().Remove(file);
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new Exception("File is still referenced or related to a nouveaute.");
        }
    }
    public async Task DeleteFileListAsync(string nouveauteId)
    {
        var fichiers = await _context.Fichiers.Where(f => f.NouveauteId == nouveauteId).ToListAsync();
        foreach (var fichier in fichiers)
        {
            bool isUsedElsewhere = await IsFileUsedElsewhereAsync(fichier.IdFichier, nouveauteId);
            if (!isUsedElsewhere)
            {
                _context.Fichiers.Remove(fichier);
            }
            else
            {
                fichier.NouveauteId = null;
                _context.Fichiers.Update(fichier);
            }
        }
        await _context.SaveChangesAsync();
    }
    private async Task<bool> IsFileUsedElsewhereAsync(string fileId, string? NouveauteId = null)
    {
        bool usedInLivres = await _context.Livres.AnyAsync(e => e.couverture == fileId);
        bool usedInNouveautesCover = await _context.Nouveautes
            .AnyAsync(e => e.couverture == fileId && e.id_nouv != NouveauteId);
        bool usedInNouveautesFile = await _context.Nouveautes
            .AnyAsync(e => e.fichier == fileId && e.id_nouv != NouveauteId);

        return usedInLivres || usedInNouveautesCover || usedInNouveautesFile;
    }
    private async Task<Fichier?> ExistingFichierAsync(string contentHash)
    {
        return await _context.Fichiers.FirstOrDefaultAsync(f => f.ContentHash == contentHash);
    }

}
