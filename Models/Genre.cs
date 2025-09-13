using System.ComponentModel.DataAnnotations;

namespace BdmiAPI.Models
{
    public class Genre : BaseEntity
    {
        [Required, StringLength(50)]
        public string Name { get; set; } = default!;

        [StringLength(500)]
        public string? Description { get; set; }

        public ICollection<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();
    }
}
