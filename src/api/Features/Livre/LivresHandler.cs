using domain.Entity.Enum;
using domain.Interfaces;
using Mapster;

namespace api.Features.Livre;

public class LivresHandler
{
    private readonly ILivresRepository _livresRepository;
    private readonly IFichierRepository _FichierRepository;

    public LivresHandler(ILivresRepository livresRepository, IFichierRepository FichierRepository)

    {
        _livresRepository = livresRepository;
        _FichierRepository = FichierRepository;
    }

    public async Task<IEnumerable<LivreDTO>> SearchAsync(string searchTerm)
    {
        try
        {
            var list = await _livresRepository.GetAllLivresAsync();
            if (searchTerm == "") return list;
            var query = list.Where(l =>
              (l.titre != null && l.titre.Contains(searchTerm)) ||
              (l.auteur != null && l.auteur.Contains(searchTerm)) ||
              (l.isbn != null && l.isbn.Contains(searchTerm)) ||
              (l.date_edition != null && l.date_edition.Contains(searchTerm)) ||
              (l.Description != null && l.Description.Contains(searchTerm)) ||
              (l.cote_liv != null && l.cote_liv.Contains(searchTerm))
            );
            return query.Select(x => x.Adapt<LivreDTO>());

        }
        catch (Exception ex)
        {
            throw new Exception($"Error searching Livres: {ex.Message}", ex);
        }
    }
    public async Task<IEnumerable<LivreDTO>> GetAllAsync()
    {
        var entities = await _livresRepository.GetAllLivresAsync();
        var dtos = entities.Adapt<IEnumerable<LivreDTO>>();
        foreach (var dto in dtos)
        {
            if (!string.IsNullOrWhiteSpace(dto.couverture))
            {
                var fichierDto = await _FichierRepository.GetFullFileInfoAsync(dto.couverture);
                dto.CouvertureFile = fichierDto;
            }
        }
        return dtos;

    }
    public async Task<LivreDTO> GetByIdAsync(string id)
    {
        var entity = await _livresRepository.GetByIdAsync(id);
        return entity.Adapt<LivreDTO>();
    }
    public async Task<LivreDTO> CreateAsync(CreateLivreRequest livredto)
    {
        if (string.IsNullOrEmpty(livredto.titre) ||
    string.IsNullOrEmpty(livredto.editeur) ||
    string.IsNullOrEmpty(livredto.date_edition) ||
    string.IsNullOrEmpty(livredto.cote_liv))
        {
            throw new Exception("Les 4 champs sont obligatoires : titre, éditeur, date édition, cote liv.");
        }
        var createdLivre = await _livresRepository.CreateAsync(livredto);

        return createdLivre.Adapt<LivreDTO>();
    }
    public async Task<LivreDTO> UpdateAsync(string id, UpdateLivreDTO UpdateLivreDTO)
    {
        var update = await _livresRepository.UpdateAsync(id, UpdateLivreDTO);
        return update.Adapt<LivreDTO>();
    }
    public async Task DeleteAsync(string id)
    {
        await _livresRepository.DeleteAsync(id);
    }
    public async Task<IEnumerable<LivreDTO>> FiltreStautLiv(Statut_liv? statut_Liv)
    {
        var liv = await GetAllAsync();
        if (statut_Liv == null) return liv;
        return liv.Where(r => r.statut == statut_Liv);
    }
}
