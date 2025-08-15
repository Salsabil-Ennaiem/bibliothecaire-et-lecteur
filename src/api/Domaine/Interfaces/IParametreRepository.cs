using domain.Entity;

namespace domain.Interfaces
{
    public interface IParametreRepository
    {
       // Task<Parametre> GetParam(string userId);
       Task<Parametre> GetParam();
        Task<Parametre> Updatepram(Parametre entity);

    }
}
