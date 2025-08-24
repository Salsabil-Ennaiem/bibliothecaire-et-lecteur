using api.Features.Livre;

namespace domain.Interfaces
{
    public interface ILivresRepository
    {
        //Task<IEnumerable<(Livres,Inventaire)>> GetAllLivresAsync();
        Task<IEnumerable<LivreDTO>> GetAllLivresAsync();
        Task<LivreDTO> GetByIdAsync(string id);
        Task<LivreDTO> CreateAsync(CreateLivreRequest livreCreate);
        Task<LivreDTO> UpdateAsync(string id, UpdateLivreDTO updatelivReq);
        Task DeleteAsync(string id);

    }
}