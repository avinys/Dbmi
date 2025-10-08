using BdmiAPI.Database;
using BdmiAPI.DTOs;
using BdmiAPI.Models;
using BdmiAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public sealed class AuthService : IAuthService
{
    private readonly AppDb _db;
    private readonly JwtSettings _jwtSettings;

    public AuthService(AppDb db, IOptions<JwtSettings> jwtOptions)
    {
        _db = db;
        _jwtSettings = jwtOptions.Value;
    }

    public async Task<TokenResponseDto> LoginAsync(LoginDto dto, CancellationToken ct = default)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username, ct);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials.");

        return await GenerateTokensAsync(user, ct);
    }

    public async Task<TokenResponseDto> RegisterAsync(RegisterDto dto, CancellationToken ct = default)
    {
        if (await _db.Users.AnyAsync(u => u.Username == dto.Username || u.Email == dto.Email, ct))
            throw new InvalidOperationException("Username or email already exists.");

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = UserRole.User
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);

        return await GenerateTokensAsync(user, ct);
    }

    public async Task<TokenResponseDto> RefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        var tokenEntity = await _db.RefreshTokens.Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && !rt.IsRevoked, ct);

        if (tokenEntity == null || tokenEntity.ExpiresAt < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Invalid refresh token.");

        tokenEntity.IsRevoked = true;
        await _db.SaveChangesAsync(ct);

        return await GenerateTokensAsync(tokenEntity.User, ct);
    }

    public async Task RevokeTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        var tokenEntity = await _db.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken, ct);
        if (tokenEntity != null)
        {
            tokenEntity.IsRevoked = true;
            await _db.SaveChangesAsync(ct);
        }
    }

    private async Task<TokenResponseDto> GenerateTokensAsync(User user, CancellationToken ct)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenLifetimeMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        // Generate refresh token
        var refreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString(),
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenLifetimeDays),
            UserId = user.Id
        };
        _db.RefreshTokens.Add(refreshToken);
        await _db.SaveChangesAsync(ct);

        return new TokenResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            ExpiresAt = expires
        };
    }
}