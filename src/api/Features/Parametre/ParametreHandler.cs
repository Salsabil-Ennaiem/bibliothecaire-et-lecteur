using domain.Entity;
using domain.Interfaces;

namespace api.Features.Parametre;

public class ParametreHandler
{
    private readonly IParametreRepository _parametreRepository;
    public ParametreHandler(IParametreRepository parametreRepository)
    {
        _parametreRepository = parametreRepository;

    }

    public async Task<ParametreDTO> GetParam()
    {
        return await _parametreRepository.GetParam();
    }
    public async Task<ParametreDTO> Updatepram(UpdateParametreDTO entity)
    {
        return await _parametreRepository.Updatepram(entity);

    }

}