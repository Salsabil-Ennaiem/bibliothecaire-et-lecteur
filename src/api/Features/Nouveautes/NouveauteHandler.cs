using domain.Entity;
using domain.Interfaces;
using Mapster;

namespace api.Features.Nouveautes
{
    public class NouveauteHandler
    {
        private readonly IRepository<Nouveaute>  _nouveauteRepository;
        public NouveauteHandler(IRepository<Nouveaute> nouveauteRepository)
        {
            _nouveauteRepository = nouveauteRepository;
        }

        public async Task<IEnumerable<NouveauteGetALL>> GetAllNouvAsync()
        {
            var rt = await _nouveauteRepository.GetAllAsync();
            return rt.Adapt<IEnumerable<NouveauteGetALL>>();

        }

        public async Task<NouveauteDTO> GetByIdAsync(string id)
        {
            var entity = await _nouveauteRepository.GetByIdAsync(id);
            return entity.Adapt<NouveauteDTO>();
        }
        public async Task<NouveauteDTO> CreateAsync(CreateNouveauteRequest createNouveaute)
        {
            var entity = createNouveaute.Adapt<Nouveaute>();
            var created = await _nouveauteRepository.CreateAsync(entity);
            return created.Adapt<NouveauteDTO>();
        }
        public async Task<NouveauteDTO> UpdateAsync(CreateNouveauteRequest nouveaute, string id)
        {
            var entity = nouveaute.Adapt<Nouveaute>();
            var Updated = await _nouveauteRepository.UpdateAsync(entity, id);
            return Updated.Adapt<NouveauteDTO>();
        }
        public async Task DeleteAsync(string id)
        {
            await _nouveauteRepository.DeleteAsync(id);
        }
    }   
}