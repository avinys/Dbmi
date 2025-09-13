using System.ComponentModel.DataAnnotations;

namespace BdmiAPI.DTOs
{
    public record MovieListItemDto(
    int Id,
    string Title,
    int ReleaseYear,
    int DurationMin,
    IReadOnlyList<string> Genres,
    double AverageScore // computed from Reviews
);

    public record MovieDetailsDto(
        int Id,
        string Title,
        string Description,
        int ReleaseYear,
        int DurationMin,
        int UploadedByUserId,
        IReadOnlyList<int> GenreIds,
        double AverageScore
    );

    public sealed class CreateMovieDto
    {
        [Required, StringLength(200)]
        public string Title { get; set; } = default!;

        [Required, StringLength(1000)]
        public string Description { get; set; } = default!;

        [Range(1888, 2100)]
        public int ReleaseYear { get; set; }

        [Range(1, 600)]
        public int DurationMin { get; set; }

        [Required] public int UploadedByUserId { get; set; }

        // optional: allow empty list
        public List<int> GenreIds { get; set; } = new();
    }

    public sealed class UpdateMovieDto
    {
        [Required, StringLength(200)]
        public string Title { get; set; } = default!;

        [Required, StringLength(1000)]
        public string Description { get; set; } = default!;

        [Range(1888, 2100)]
        public int ReleaseYear { get; set; }

        [Range(1, 600)]
        public int DurationMin { get; set; }

        public List<int> GenreIds { get; set; } = new();
    }

}

