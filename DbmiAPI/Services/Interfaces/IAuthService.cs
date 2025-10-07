public interface IAuthService
{
    Task<TokenResponseDto> LoginAsync(LoginDto dto, CancellationToken ct = default);
    Task<TokenResponseDto> RegisterAsync(RegisterDto dto, CancellationToken ct = default);
    Task<TokenResponseDto> RefreshTokenAsync(string refreshToken, CancellationToken ct = default);
    Task RevokeTokenAsync(string refreshToken, CancellationToken ct = default);
}