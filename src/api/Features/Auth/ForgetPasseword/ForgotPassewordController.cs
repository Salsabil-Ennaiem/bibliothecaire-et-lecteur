using api.Features.Auth.ForgetPassword;
using Microsoft.AspNetCore.Mvc;

namespace api.Features.Auth.ForgetPasseword;

[ApiController]
[Route("api/[controller]")]
public class ForgotPassewordController : ControllerBase
{
    private readonly ForgotPasswordHandler _forgotPasswordHandler;

    public ForgotPassewordController(ForgotPasswordHandler forgotPasswordHandler)
    {
        _forgotPasswordHandler = forgotPasswordHandler;
    }
    [HttpPost("MDPOubliee")]
    public async Task<IActionResult> MDPOubliee([FromBody] ForgotPasswordRequestDto request)
    {
        var command = new ForgotPasswordRequestDto(request.Email);
        var response = await _forgotPasswordHandler.Handle(command);
        return Ok(response);
    }
    [HttpPost("reset-password")]
    public async Task<IResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
    {
        var command = new ResetPasswordRequestDto(request.Email, request.Token, request.NewPassword);
        return await _forgotPasswordHandler.HandleResetPassword(command);
    }
}
