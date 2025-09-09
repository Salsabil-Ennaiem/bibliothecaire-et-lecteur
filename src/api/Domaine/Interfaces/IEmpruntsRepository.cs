using api.Features.Emprunt;

namespace domain.Interfaces

{
    public interface IEmpruntsRepository 
    {
        Task<IEnumerable<EmppruntDTO>> GetAllEmpAsync();
        Task<EmppruntDTO> GetByIdAsync(string id);
        Task<EmppruntDTO> CreateAsync(CreateEmpRequest empdto);
        Task<EmppruntDTO> UpdateAsync(string id, UpdateEmppruntDTO updateEmpReq);
        Task DeleteAsync(string id);

    }
}
