namespace DbmiAPI.Models
{
    public class MovieGenre
    {
        // composite key (MovieId, GenreId)
        public int MovieId { get; set; }
        public Movie Movie { get; set; } = default!;

        public int GenreId { get; set; }
        public Genre Genre { get; set; } = default!;
    }
}
