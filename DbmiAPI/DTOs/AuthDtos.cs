public sealed class LoginDto
{
    [Required]
    public string Username { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}

public sealed class RegisterDto
{
    [Required, StringLength(40)]
    public string Username { get; set; } = string.Empty;
    
    [Required, EmailAddress, StringLength(256)]
    public string Email { get; set; } = string.Empty;
    
    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;
}

public sealed class TokenResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

public sealed class RefreshTokenDto
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}