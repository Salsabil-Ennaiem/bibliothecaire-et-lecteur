using domain.Entity;
using domain.Interfaces;
using Mapster;

namespace api.Features.Nouveautes
{
    public class NouveauteHandler
    {
        private readonly IRepository<Nouveaute> _nouveauteRepository;
        private readonly IFichierRepository _FichierRepository;
        public NouveauteHandler(IRepository<Nouveaute> nouveauteRepository, IFichierRepository FichierRepository)
        {
            _nouveauteRepository = nouveauteRepository;
            _FichierRepository = FichierRepository;
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
        public async Task<NouveauteDTO> CreateAsync(CreateNouveauteRequestWithFiles createdNouveauteDto)
        {
                var nouv = createdNouveauteDto.Adapt<Nouveaute>();
                var file = createdNouveauteDto.Adapt<List<Fichier>>();
            var couv = createdNouveauteDto.Adapt<Fichier>();
            if (!await ExistenceNouv(nouv.titre))
            {
                var id = nouv.id_nouv;

                if (file != null)
                {

                    if (file.Count() == 1)
                    {
                        var filecree = await _FichierRepository.UploadF(file, id);
                        nouv.fichier = filecree.IdFichier;
                    }
                    else
                    {
                        await _FichierRepository.UploadF(file, id);
                    }

                }
                if (couv != null)
                {
                    var filecree = await _FichierRepository.UploadImageAsync(couv);
                    nouv.couverture = filecree;
                }
                var created = await _nouveauteRepository.CreateAsync(nouv);
                return created.Adapt<NouveauteDTO>();

            }
            throw new Exception("Alredy exist");
        }
        public async Task<NouveauteDTO> UpdateAsync(UpdateNouveauteRequest nouveaute, string id)
        {
            if (!await ExistenceNouv(nouveaute.titre))
            {
                var entity = nouveaute.Adapt<Nouveaute>();
                var Updated = await _nouveauteRepository.UpdateAsync(entity, id);
                return Updated.Adapt<NouveauteDTO>();
            } 
            throw new Exception("Alredy exist");
        }
        public async Task DeleteAsync(string id)
        {
            var nouv = await GetByIdAsync(id);
            await _FichierRepository.DeleteFileByIdAsync(nouv.couverture);
            await _FichierRepository.DeleteFileByIdAsync(nouv.fichier);
            await _FichierRepository.DeleteFileListAsync(id);
            await _nouveauteRepository.DeleteAsync(id);
        }
        private async Task<bool> ExistenceNouv(string titre)
        {
            var allNouveautes = await GetAllNouvAsync();
            return allNouveautes.Any(n => n.titre == titre);
        }

    }
}