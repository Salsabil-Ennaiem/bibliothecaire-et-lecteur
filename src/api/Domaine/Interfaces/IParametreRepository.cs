using api.Features.Parametre;
using domain.Entity;
using domain.Entity.Enum;

namespace domain.Interfaces
{
    public interface IParametreRepository
    {
        // Task<Parametre> GetParam(string userId);
        Task<ParametreDTO> GetParam();
        Task<ParametreDTO> Updatepram(UpdateParametreDTO entity);
        Task<int> GetDelais(TypeMemb type);

    }
}
