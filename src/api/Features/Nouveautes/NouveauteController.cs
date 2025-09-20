using domain.Entity;
using Microsoft.AspNetCore.Mvc;

namespace api.Features.Nouveautes
{

    [ApiController]
    [Route("api/[controller]")]
    public class NouveauteController : ControllerBase
    {
        private readonly NouveauteHandler _nouveauteHandler;

        public NouveauteController(NouveauteHandler nouveauteHandler)
        {
            _nouveauteHandler = nouveauteHandler;
        }
        [HttpGet("Getall")]
        public async Task<IActionResult> GetAllNouv()
        {
            var nouveautes = await _nouveauteHandler.GetAllNouvAsync();
            return Ok(nouveautes);
        }

        [HttpGet("Get/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var nouveautes = await _nouveauteHandler.GetByIdAsync(id);
            return Ok(nouveautes);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateNouveauteRequestWithFiles createdNouveauteDto)
        {
            var createdNouveaute = await _nouveauteHandler.CreateAsync(createdNouveauteDto );
            return Ok(createdNouveaute); 
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _nouveauteHandler.DeleteAsync(id);
            return NoContent();
        }
    }
}