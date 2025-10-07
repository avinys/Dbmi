using BdmiAPI.DTOs;

namespace BdmiAPI.Services.Interfaces
{
    public interface IMovieService
    {
        Task<IReadOnlyList<MovieListItemDto>> ListAsync(int? genreId, string? q, CancellationToken ct = default);
        Task<MovieDetailsDto?> GetAsync(int id, CancellationToken ct = default);
        Task<MovieDetailsDto> CreateAsync(CreateMovieDto dto, CancellationToken ct = default);
        Task<bool> UpdateAsync(int id, UpdateMovieDto dto, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);

        // hierarchical
        Task<object?> GetReviewsForMovieAsync(int id, bool includeText, CancellationToken ct = default);
    }

    public sealed class MovieConflictException : Exception
    {
        public MovieConflictException(string m) : base(m) { }
    }
    public sealed class MovieValidationException : Exception
    {
        public MovieValidationException(string m) : base(m) { }
    }

}

