using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace BdmiAPI.Models
{
    public class Movie : BaseEntity
    {
        [Required, StringLength(200)]
        public string Title { get; set; } = default!;

        [Required, StringLength(1000)]
        public string Description { get; set; } = default!;

        [Range(1888, 2100)]
        public int ReleaseYear { get; set; }

        [Range(1, 600)]
        public int DurationMin { get; set; }

        public MovieStatus Status { get; set; } = MovieStatus.Pending;

        [Range(0, 10)]
        public double AverageScore { get; set; }

        public int UploadedByUserId { get; set; }
        public User UploadedByUser { get; set; } = default!;

        // Navigation
        public ICollection<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
