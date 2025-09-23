using Microsoft.AspNetCore.Mvc;

namespace api.Features.Auth.Login;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly LoginHandler _loginHandler;
    public LoginController(LoginHandler loginHandler)
    {
        _loginHandler = loginHandler;
    }
         [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var response = await _loginHandler.LoginAsync(request);
        return Ok(response);
    }
}
