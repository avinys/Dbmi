using BdmiAPI.DTOs;

namespace BdmiAPI.Services.Interfaces
{
    public interface IReviewService
    {
        Task<IReadOnlyList<ReviewListItemDto>> ListAsync(int? movieId, int? userId, CancellationToken ct = default);
        Task<ReviewDetailsDto?> GetAsync(int id, CancellationToken ct = default);
        Task<ReviewDetailsDto> CreateAsync(CreateReviewDto dto, CancellationToken ct = default);
        Task<bool> UpdateAsync(int id, UpdateReviewDto dto, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
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
