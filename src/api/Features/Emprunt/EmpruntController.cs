using domain.Entity.Enum;
using Microsoft.AspNetCore.Mvc;

namespace api.Features.Emprunt;

[ApiController]
[Route("api/[controller]")]

public class EmpruntController : ControllerBase
{
    private readonly EmpruntHandler _empruntHundler;
    public EmpruntController(EmpruntHandler empruntHundler)
    {
        _empruntHundler = empruntHundler;
    }

    [HttpGet("Getall")]
    public async Task<IActionResult> GetAll()
    {
        var livres = await _empruntHundler.GetAllAsync();
        return Ok(livres);
    }
    [HttpPost("Create/{id}")]
    public async Task<IActionResult> Create([FromBody] CreateEmpRequest emp , string id)
    {
        var createdLivre = await _empruntHundler.CreateAsync(id , emp);
        return Ok(createdLivre);
    }
    [HttpGet("Get/{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var livre = await _empruntHundler.GetByIdAsync(id);
        return Ok(livre);
    }
    [HttpPatch("Update/{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateEmppruntDTO emp)
    {
        var updated = await _empruntHundler.UpdateAsync(id,emp);
        return Ok(updated);
    }

    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _empruntHundler.DeleteAsync(id);
        return NoContent();
    }
    [HttpGet("search/{term}")]
    public async Task<IActionResult> Search(string term)
    {
        var results = await _empruntHundler.SearchAsync(term);
        return Ok(results);
    }
     [HttpGet("FiltrEmp/{Statut}")]

    public async Task<IActionResult> FiltreEmp(Statut_emp? Statut)
    {
        var rst= await _empruntHundler.FiltreStautEmp(Statut);
                return Ok(rst);

    }
    
    [HttpGet("Notification")]
    public async Task<IActionResult> GetNotifications()
    {
        await _empruntHundler.GererAlertesEtNotificationsAsync();
        return Ok("Notifications sent successfully.");
    }
   
}
