namespace BdmiAPI.DTOs
{
    public class ReviewForMovieDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Score { get; set; }
        public string? Title { get; set; }
        public string? Body { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class MovieReviewsDto
    {
        public int MovieId { get; set; }
        public IReadOnlyList<ReviewForMovieDto> Reviews { get; set; } = Array.Empty<ReviewForMovieDto>();
    }
}
