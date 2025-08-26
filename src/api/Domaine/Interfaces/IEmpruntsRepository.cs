using api.Features.Emprunt;
using domain.Entity;

namespace domain.Interfaces

{
    public interface IEmpruntsRepository 
    {
        Task<IEnumerable<Emprunts>> GetOverdueEmpruntsAsync(string userId, DateTime currentDate);
        Task<IEnumerable<EmppruntDTO>> GetAllEmpAsync();
        Task<EmppruntDTO> GetByIdAsync(string id);
       // Task<EmppruntDTO> CreateAsync(CreateLivreRequest livreCreate);
       // Task<EmppruntDTO> UpdateAsync(string id, UpdateLivreDTO updatelivReq);
        Task DeleteAsync(string id);

    }
}
