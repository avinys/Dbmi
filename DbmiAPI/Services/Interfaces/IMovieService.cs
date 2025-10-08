using System.Security.Claims;
using BdmiAPI.DTOs;

namespace BdmiAPI.Services.Interfaces
{
    public interface IMovieService
    {
        Task<IReadOnlyList<MovieListItemDto>> ListAsync(int? genreId, string? q, ClaimsPrincipal user, CancellationToken ct = default);
        Task<MovieDetailsDto?> GetAsync(int id, ClaimsPrincipal user, CancellationToken ct = default);
        Task<MovieDetailsDto> CreateAsync(CreateMovieDto dto, ClaimsPrincipal user, CancellationToken ct = default);
        Task<bool> UpdateAsync(int id, UpdateMovieDto dto, ClaimsPrincipal user, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, ClaimsPrincipal user, CancellationToken ct = default);
        Task<bool> ApproveAsync(int id, ClaimsPrincipal user, CancellationToken ct = default);
        Task<bool> RejectAsync(int id, ClaimsPrincipal user, CancellationToken ct = default);
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

