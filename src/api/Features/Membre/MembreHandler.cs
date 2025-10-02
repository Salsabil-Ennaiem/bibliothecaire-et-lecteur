using api.Features.Emprunt;
using domain.Entity;
using domain.Entity.Enum;
using domain.Interfaces;
using Mapster;

namespace api.Features.Membres;

public class MembreHandler
{
    private readonly IRepository<Membre> _MembreRepository;
    private readonly EmpruntHandler _EmpruntHandler;
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
        var Memb = await _MembreRepository.GetByIdAsync(id);
        if (membre.nom != Memb.nom) Memb.nom = membre.nom;
        if (membre.prenom != Memb.prenom) Memb.prenom = membre.prenom;
        if (membre.email != Memb.email) Memb.email = membre.email;
        if (membre.telephone != Memb.telephone) Memb.telephone = membre.telephone;
        if (membre.TypeMembre != Memb.TypeMembre) Memb.TypeMembre = membre.TypeMembre;
        var Updated = await _MembreRepository.UpdateAsync(Memb);
        return Updated.Adapt<MembreDto>();
    }
    public async Task DeleteAsync(string id)
    {
        var memb = await _MembreRepository.GetByIdAsync(id);
        var emp = await _EmpruntHandler.SearchAsync(id);
        var statut = emp.First().Statut_emp;
        bool estEnCours = statut == Statut_emp.en_cours;
        if (memb.Statut == StatutMemb.actif && !estEnCours)
        { await _MembreRepository.DeleteAsync(id); }
        else throw new Exception("seulment mebre non des emprunt en cours  et sont active  ");

    }
    public async Task<IEnumerable<MembreDto>> SearchAsync(string searchTerm)
    {
        var list = await GetAllMembAsync();
        if (searchTerm == "") { return list; }
        var query = list.Where(m => (m.cin_ou_passeport != null && m.cin_ou_passeport == searchTerm)
                           || (m.email != null && m.email.Contains(searchTerm))
                           || (m.nom != null && m.nom.Contains(searchTerm))
                           || (m.prenom != null && m.prenom.Contains(searchTerm)));
        return query;
    }
    public async Task<IEnumerable<MembreDto>> FiltreStautMemb(StatutMemb? statut_Memb)
    {
        var Memb = await GetAllMembAsync();
        if (statut_Memb == null) return Memb;
        return Memb.Where(r => r.Statut == statut_Memb);
    }

}
