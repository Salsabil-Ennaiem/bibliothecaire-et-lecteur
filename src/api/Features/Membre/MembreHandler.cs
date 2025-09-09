using domain.Entity;
using domain.Interfaces;
using Mapster;

namespace api.Features.Membres;

public class MembreHandler
{
    private readonly IRepository<Membre> _MembreRepository;
    public MembreHandler(IRepository<Membre> MembreRepository)
    {
        _MembreRepository = MembreRepository;
    }

    public async Task<IEnumerable<MembreDto>> GetAllMembAsync()
    {
        var rt = await _MembreRepository.GetAllAsync();
        return rt.Adapt<IEnumerable<MembreDto>>();

    }

    public async Task<MembreDto> GetByIdAsync(string id)
    {
        var entity = await _MembreRepository.GetByIdAsync(id);
        return entity.Adapt<MembreDto>();
    }

    public async Task<MembreDto> UpdateAsync(UpdateMembreDto membre, string id)
    {

        var entity = membre.Adapt<Membre>();
        var Updated = await _MembreRepository.UpdateAsync(entity, id);
        return Updated.Adapt<MembreDto>();
    }
    public async Task DeleteAsync(string id)
    {
        await _MembreRepository.DeleteAsync(id);
    }
    public async Task<IEnumerable<MembreDto>> SearchAsync(string searchTerm)
    {
        var list = await GetAllMembAsync();
        var query = list.Where(m=>(m.cin_ou_passeport != null && m.cin_ou_passeport.Contains(searchTerm))
                           || (m.email != null && m.email.Contains(searchTerm))
                           || (m.id_membre != null && m.id_membre.Contains(searchTerm))
                           || (m.nom != null && m.nom.Contains(searchTerm))
                           || (m.prenom != null && m.prenom.Contains(searchTerm)));
        return query;
    }
}
