using domain.Entity;

namespace domain.Interfaces
{
    public interface ILivresRepository
    {
        Task<IEnumerable<(Livres,Inventaire)>> GetAllLivresAsync();
        Task<(Livres, Inventaire)> GetByIdAsync(string id);
        Task<(Livres ,Inventaire , string)> CreateAsync(Livres livre, Inventaire inventaire);
        Task<(Livres ,Inventaire)> UpdateAsync(string id, Livres livre, Inventaire inventaire );
        Task DeleteAsync(string id);

    }
}