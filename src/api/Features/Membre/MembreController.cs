using Microsoft.AspNetCore.Mvc;

namespace api.Features.Membres;


[ApiController]
[Route("api/[controller]")]
public class MembreController : ControllerBase
{
    private readonly MembreHandler _MembreHandler;

    public MembreController(MembreHandler MembreHandler)
    {
        _MembreHandler = MembreHandler;
    }
    [HttpGet("Getall")]
    public async Task<IActionResult> GetAllMemb()
    {
        var membre = await _MembreHandler.GetAllMembAsync();
        return Ok(membre);
    }

    [HttpGet("Get/{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var membre = await _MembreHandler.GetByIdAsync(id);
        return Ok(membre);
    }


    [HttpPut("Update/{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateMembreDto request)
    {
        var updated = await _MembreHandler.UpdateAsync(request, id);
        return Ok(updated);
    }

    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _MembreHandler.DeleteAsync(id);
        return NoContent();
    }
        [HttpGet("search/{term}")]
    public async Task<IActionResult> Search(string term)
    {
        var results = await _MembreHandler.SearchAsync(term);
        return Ok(results);
    }
}