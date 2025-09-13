using System.ComponentModel.DataAnnotations;

namespace DbmiAPI.Models
{
    public class Genre : BaseEntity
    {
        [Required, StringLength(50)]
        public string Name { get; set; } = default!;

        public ICollection<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();
    }
}
