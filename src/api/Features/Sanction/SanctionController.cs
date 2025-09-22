using domain.Entity.Enum;
using Microsoft.AspNetCore.Mvc;

namespace api.Features.Sanctions;

[ApiController]
[Route("api/[controller]")]
//[Authorize]
public class SanctionController : ControllerBase
{
    private readonly SanctionHandler _sanctionHandler;


    public SanctionController(SanctionHandler sanctionHandler)
    {
        _sanctionHandler = sanctionHandler;
    }

    [HttpGet("search/{term}")]
    public async Task<IActionResult> Search(string term)
    {
        var results = await _sanctionHandler.SearchAsync(term);
        return Ok(results);
    }
    [HttpGet("FiltrPay/{paye}")]

    public async Task<IActionResult> FiltrePayement(bool? paye)
    {
        var rt = await _sanctionHandler.FiltrePayement(paye);
                return Ok(rt);

    }
[HttpGet("FiltrRaison")]
public async Task<IActionResult> FiltreRaison([FromQuery] Raison_sanction[]? raison)
{
    var rst = await _sanctionHandler.FiltreRaison(raison);
    return Ok(rst);
}


    [HttpGet("Getall")]
    public async Task<IActionResult> GetAll()
    {
        var sanctions = await _sanctionHandler.GetAllAsync();
        return Ok(sanctions);
    }

    [HttpPost("Create/{id}")]
    public async Task<IActionResult> Create([FromBody] CreateSanctionRequest createSanction, string id)
    {
        var createdSanc = await _sanctionHandler.CreateAsync(createSanction, id);
        return Ok(createdSanc);
    }
    [HttpPatch("modifier/{id}")]
    public async Task Modifier(string id)
    {
        await _sanctionHandler.ModifierAsync(id);
    }
}
