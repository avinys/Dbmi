using BdmiAPI.DTOs;
using System.Security.Claims;

namespace BdmiAPI.Services.Interfaces
{
    public interface IReviewService
    {
        Task<IReadOnlyList<ReviewListItemDto>> ListAsync(int? movieId, int? userId, ClaimsPrincipal user, CancellationToken ct = default);
        Task<ReviewDetailsDto?> GetAsync(int id, ClaimsPrincipal user, CancellationToken ct = default);
        Task<ReviewDetailsDto> CreateAsync(CreateReviewDto dto, ClaimsPrincipal user, CancellationToken ct = default);
        Task<bool> UpdateAsync(int id, UpdateReviewDto dto, ClaimsPrincipal user, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, ClaimsPrincipal user, CancellationToken ct = default);
    }

    public sealed class ReviewConflictException : Exception
    {
        public ReviewConflictException(string message) : base(message) { }
    }

    public sealed class ReviewValidationException : Exception
    {
        public ReviewValidationException(string message) : base(message) { }
    }
}
