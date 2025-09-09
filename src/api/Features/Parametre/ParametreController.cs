using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Features.Parametre;

[ApiController]
[Route("api/[controller]")]

public class ParametreController : ControllerBase
{
    private readonly ParametreHandler _parametreHandler;
    public ParametreController(ParametreHandler parametreHandler)
    {
        _parametreHandler = parametreHandler;
    }
    [HttpGet("Get")]
    public async Task<IActionResult> GetById()
    {
        var Parametre = await _parametreHandler.GetParam();
        return Ok(Parametre);
    }
    [HttpPost("Update")]
    public async Task<IActionResult> Update([FromBody] UpdateParametreDTO Parametre)
    {
        var Updatepram = await _parametreHandler.Updatepram(Parametre);
        return Ok(Updatepram);
    }
}
