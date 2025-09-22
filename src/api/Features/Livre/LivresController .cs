using domain.Entity.Enum;
using Microsoft.AspNetCore.Mvc;

namespace api.Features.Livre;
[ApiController]
[Route("api/[controller]")]
public class LivresController : ControllerBase
{
    private readonly LivresHandler _livresHandler;


    public LivresController(LivresHandler livresHandler)
    {
        _livresHandler = livresHandler;
    }

    [HttpGet("search/{term}")]
    public async Task<IActionResult> Search(string term)
    {
        var results = await _livresHandler.SearchAsync(term);
        return Ok(results);
    }

    [HttpGet("Getall")]
    public async Task<IActionResult> GetAll()
    {
        var livres = await _livresHandler.GetAllAsync();
        return Ok(livres);
    }
    [HttpGet("Get/{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var livre = await _livresHandler.GetByIdAsync(id);
        return Ok(livre);
    }


    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] CreateLivreRequest livre)
    {
        var createdLivre = await _livresHandler.CreateAsync(livre);
        return Ok(createdLivre);
    }


    [HttpPut("Update/{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateLivreDTO livre)
    {
        var updated = await _livresHandler.UpdateAsync(id , livre);
        return Ok(updated);
    }

    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _livresHandler.DeleteAsync(id);
        return NoContent();
    }
 [HttpGet("FiltrLiv/{statut}")]

    public async Task<IActionResult> FiltreRaison(Statut_liv? statut)
    {
        var rst= await _livresHandler.FiltreStautLiv(statut);
                return Ok(rst);

    }
}