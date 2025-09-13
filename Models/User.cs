namespace DbmiAPI.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Linq;
    using Microsoft.AspNetCore.Mvc.ViewEngines;

    public class User : BaseEntity
    {
        [Required, StringLength(40)]
        public string Username { get; set; } = default!;

        [Required, StringLength(256)]
        public string Email { get; set; } = default!;

        [StringLength(255)]
        public string? PasswordHash { get; set; }

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Movie> UploadedMovies { get; set; } = new List<Movie>();
    }
}
