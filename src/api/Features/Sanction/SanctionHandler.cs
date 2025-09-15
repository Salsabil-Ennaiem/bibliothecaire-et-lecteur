using domain.Entity;
using domain.Interfaces;
using Infrastructure.Repositries;
using Mapster;

namespace api.Features.Sanctions;

public class SanctionHandler
{
    private readonly IRepository<Sanction> _sanctionRepository;


    public SanctionHandler(Repository<Sanction> sanctionRepository)
    {
        _sanctionRepository = sanctionRepository;
    }


    public async Task<IEnumerable<SanctionDTO>> GetAllAsync()
    {
        var entities = await _sanctionRepository.GetAllAsync();
        return entities.Adapt<IEnumerable<SanctionDTO>>();
    }
    public async Task<SanctionDTO> CreateAsync(CreateSanctionRequest createSanction, string id)
    {
        var entity = createSanction.Adapt<Sanction>();
        if (entity.raison == null || entity.date_fin_sanction == null || entity.date_fin_sanction > DateTime.Now)
        {
            throw new Exception("Raison et date fin sont obligatoire ainsi date fin doit > date aujourd'hui ");
        }
        entity.id_emp = id;
        entity.payement = entity.montant > 0 ? false : true;
        entity.id_sanc = Guid.NewGuid().ToString();
        var created = await _sanctionRepository.CreateAsync(entity);
        return created.Adapt<SanctionDTO>();
    }
    public async Task<IEnumerable<SanctionDTO>> SearchAsync(string searchTerm)
    {

        var list = await GetAllAsync();
        if (searchTerm == "") { return list; }
        var query = list.Where(s => (s.description != null && s.description.Contains(searchTerm))
                        || (s.raison != null && s.raison.Any(r => r.ToString().Contains(searchTerm)))
                           || (s.email != null && s.email.Contains(searchTerm)));

        return query;
    }
    public async Task ModifierAsync(string id)
    {
        try
        {
            var sanction = await _sanctionRepository.GetByIdAsync(id);
            sanction.payement = true;
            await _sanctionRepository.UpdateAsync(sanction, id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving Livre with ID {id}: {ex.Message}", ex);

        }
    }
}