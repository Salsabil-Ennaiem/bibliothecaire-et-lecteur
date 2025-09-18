using api.Features.Emprunt;
using api.Features.Membres;
using domain.Entity;
using domain.Entity.Enum;
using domain.Interfaces;
using Infrastructure.Repositries;
using Mapster;

namespace api.Features.Sanctions;

public class SanctionHandler
{
    private readonly IRepository<Sanction> _sanctionRepository;
    private readonly MembreHandler _handlerMember;
    private readonly EmpruntHandler _handlerEmp;

    public SanctionHandler(Repository<Sanction> sanctionRepository, MembreHandler hundlermember, EmpruntHandler handlerEmp)
    {
        _sanctionRepository = sanctionRepository;
        _handlerMember = hundlermember;
        _handlerEmp = handlerEmp;
    }


    public async Task<IEnumerable<SanctionDTO>> GetAllAsync()
    {
        var sanctions = await _sanctionRepository.GetAllAsync();

        var sanctionDtos = new List<SanctionDTO>();

        foreach (var sanction in sanctions)
        {
            var membre = await _handlerMember.GetByIdAsync(sanction.id_membre);
            EmppruntDTO emprunt = null;
            if (!string.IsNullOrEmpty(sanction.id_emp))
            {
                emprunt = await _handlerEmp.GetByIdAsync(sanction.id_emp);
            }

            sanctionDtos.Add(new SanctionDTO
            {
                id_sanc = sanction.id_sanc,
                id_membre = sanction.id_membre,
                id_emp = sanction.id_emp,
                email = membre.email,
                date_emp = emprunt.date_emp,
                raison = sanction.raison,
                date_sanction = sanction.date_sanction,
                date_fin_sanction = sanction.date_fin_sanction,
                montant = sanction.montant,
                payement = sanction.payement,
                active = sanction.active,
                description = sanction.description
            });
        }

        return sanctionDtos;
    }


    public async Task<SanctionDTO> CreateAsync(CreateSanctionRequest createSanction, string id)
    {
        if (createSanction.raison == null || createSanction.date_fin_sanction == null || createSanction.date_fin_sanction > DateTime.Now)
        {
            throw new Exception("Raison et date fin sont obligatoire ainsi date fin doit > date aujourd'hui ");
        }
        var emprunt = await _handlerEmp.GetByIdAsync(id);
        var entity = createSanction.Adapt<Sanction>();

        if (emprunt.Statut_emp == Statut_emp.retourne && emprunt.date_effectif <= entity.date_sanction.AddDays(1))
        { 
        entity.id_membre = emprunt.id_membre;
        entity.id_emp = id;
        entity.payement = entity.montant > 0 ? false : true;
        entity.id_sanc = Guid.NewGuid().ToString();
        var created = await _sanctionRepository.CreateAsync(entity);
        return created.Adapt<SanctionDTO>();
    }
        else throw new Exception("vous ne confirme pas le critère de Sanction");
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