using DoomscrollingAnalyzer.API.DTOs;
using DoomscrollingAnalyzer.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DoomscrollingAnalyzer.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterRequestDto request)
    {
        if (await authService.EmailExistsAsync(request.Email))
        {
            return Conflict(new { message = "Email is already registered." });
        }

        var response = await authService.RegisterAsync(request);

        return CreatedAtAction(nameof(Register), new { id = response.UserId }, response);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginRequestDto request)
    {
        var response = await authService.LoginAsync(request);

        if (response is null)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        return Ok(response);
    }
}
