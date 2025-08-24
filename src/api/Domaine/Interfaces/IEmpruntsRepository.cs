using api.Features.Emprunt;
using domain.Entity;

namespace domain.Interfaces

{
    public interface IEmpruntsRepository 
    {
        Task<IEnumerable<Emprunts>> GetOverdueEmpruntsAsync(string userId, DateTime currentDate);
        Task<IEnumerable<EmppruntDTO>> GetAllEmpAsync();
        Task<EmppruntDTO> GetByIdAsync(string id);
       // Task<string> CreateAsync(CreateLivreRequest livreCreate);
       // Task<LivreDTO> UpdateAsync(string id, UpdateLivreDTO updatelivReq);
        Task DeleteAsync(string id);

    }
}
