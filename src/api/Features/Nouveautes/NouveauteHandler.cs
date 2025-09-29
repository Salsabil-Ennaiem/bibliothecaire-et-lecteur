using api.Common;
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
            var nouveautes = await _nouveauteRepository.GetAllAsync();

            var dtos = nouveautes.Adapt<IEnumerable<NouveauteGetALL>>().ToList();

            foreach (var dto in dtos)
            {
                if (!string.IsNullOrWhiteSpace(dto.couverture))
                {
                    var fichierDto = await _FichierRepository.GetFullFileInfoAsync(dto.couverture);
                    dto.CouvertureFile = fichierDto;
                }
            }

            return dtos;
        }
        public async Task<NouveauteDTO?> GetByIdAsync(string id)
        {
            var nouveaute = await _nouveauteRepository.GetByIdAsync(id);
            if (nouveaute == null) return null;

            var dto = nouveaute.Adapt<NouveauteDTO>();

            if (!string.IsNullOrWhiteSpace(dto.couverture))
            {
                dto.CouvertureFile = await _FichierRepository.GetFullFileInfoAsync(dto.couverture);
            }

            var fichiers = await _FichierRepository.GetByNouveauteIdAsync(id);
            dto.Fichiers = fichiers.Adapt<List<FichierDto>>();

            return dto;
        }
        public async Task<NouveauteDTO> CreateAsync(CreateNouveauteRequestWithFiles createdNouveauteDto)
        {
            var nouv = createdNouveauteDto.Adapt<Nouveaute>();
            nouv.id_nouv = Guid.NewGuid().ToString();
            var files = (createdNouveauteDto.File != null) ? createdNouveauteDto.File.Adapt<List<Fichier>>() : new List<Fichier>();

            var couverture = (createdNouveauteDto.Couv != null)
                ? createdNouveauteDto.Couv.Adapt<Fichier>() : null;
            if (!await ExistenceNouv(nouv.titre))
            {
                var id = nouv.id_nouv;

                if (files.Any())
                {
                    Console.WriteLine("the there is a file  ");

                    if (files.Count() == 1)
                    {
                        var filecree = await _FichierRepository.UploadF(files, id);
                        Console.WriteLine("the fichierrfffffffffffffffffffffffffffffffffffff ", filecree);
                        nouv.fichier = filecree.IdFichier;
                        Console.WriteLine("the fichierokkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkk ", nouv.fichier);

                    }
                    else
                    {
                        await _FichierRepository.UploadF(files, id);
                        Console.WriteLine("elseeeeeeeee okkkkkkkkkkkkk");

                    }

                }

                if (couverture != null)
                {
                    var filecree = await _FichierRepository.UploadImageAsync(couverture);
                    Console.WriteLine("the coverturrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr ", filecree);
                    nouv.couverture = filecree;
                    Console.WriteLine("theeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee covertur ", nouv.couverture);

                }
                else Console.WriteLine("noooooooooooooooooooooooooooooooooooooo couvertur");
                var created = await _nouveauteRepository.CreateAsync(nouv);
                return created.Adapt<NouveauteDTO>();

            }
            throw new Exception("Alredy exist");
        }
        public async Task DeleteAsync(string id)
        {
            var nouv = await GetByIdAsync(id);
            if (nouv.couverture != null) await _FichierRepository.DeleteFileByIdAsync(nouv.couverture);
            if (nouv.fichier != null) await _FichierRepository.DeleteFileByIdAsync(nouv.fichier);
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