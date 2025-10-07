using BdmiAPI.DTOs;
using BdmiAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _svc;
    public AuthController(IAuthService svc) => _svc = svc;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken ct)
    {
        var tokens = await _svc.LoginAsync(dto, ct);
        return Ok(tokens);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto, CancellationToken ct)
    {
        var tokens = await _svc.RegisterAsync(dto, ct);
        return Ok(tokens);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto, CancellationToken ct)
    {
        var tokens = await _svc.RefreshTokenAsync(dto.RefreshToken, ct);
        return Ok(tokens);
    }

    [HttpPost("revoke")]
    public async Task<IActionResult> Revoke([FromBody] RefreshTokenDto dto, CancellationToken ct)
    {
        await _svc.RevokeTokenAsync(dto.RefreshToken, ct);
        return NoContent();
    }
}