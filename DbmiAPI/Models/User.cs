namespace BdmiAPI.Models
{
    using System.ComponentModel.DataAnnotations;

    public class User : BaseEntity
    {
        [Required, StringLength(40)]
        public string Username { get; set; } = string.Empty;
        
        [Required, StringLength(256)]
        public string Email { get; set; } = string.Empty;
        
        [StringLength(255)]
        public string? PasswordHash { get; set; }
        
        public bool IsSystem { get; set; }
        
        [Required]
        public UserRole Role { get; set; } = UserRole.User;
        
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Movie> UploadedMovies { get; set; } = new List<Movie>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
