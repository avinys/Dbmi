using System.ComponentModel.DataAnnotations;

namespace BdmiAPI.DTOs
{
    public record UserListItemDto(
        int Id,
        string Username,
        string Email,
        DateTime CreatedAt
    );

    public record UserDetailsDto(
        int Id,
        string Username,
        string Email,
        DateTime CreatedAt
    );

    public sealed class CreateUserDto
    {
        [Required, StringLength(40)]
        public string Username { get; set; } = default!;

        [Required, EmailAddress, StringLength(256)]
        public string Email { get; set; } = default!;
    }

    public sealed class UpdateUserDto
    {
        [Required, StringLength(40)]
        public string Username { get; set; } = default!;

        [Required, EmailAddress, StringLength(256)]
        public string Email { get; set; } = default!;
    }
}
