using api.Features.Livre;

namespace domain.Interfaces
{
    public interface ILivresRepository
    {
        Task<IEnumerable<LivreDTO>> GetAllLivresAsync();
        Task<LivreDTO> GetByIdAsync(string id);
        Task<LivreDTO> CreateAsync(CreateLivreRequest livreCreate);
        Task<LivreDTO> UpdateAsync(string id, UpdateLivreDTO updatelivReq);
        Task DeleteAsync(string id);
        string RechercheCote(string cote);

    }
}