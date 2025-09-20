using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Features.Profile;

[ApiController]
[Route("api/[controller]")]
[Authorize]

public class ProfileController : ControllerBase
{

    private readonly ProfileHandler _profileHandler;

    public ProfileController(ProfileHandler profileHandler)
    {
        _profileHandler = profileHandler;
    }

    [HttpGet("Get")]
    public async Task<ActionResult<ProfileDTO>> GetProfile()
    {
        var profile = await _profileHandler.GetProfileAsync();
        return Ok(profile);
    }

    [HttpPut("put")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        await _profileHandler.UpdateProfileAsync(dto);
        return NoContent();
    }
}

