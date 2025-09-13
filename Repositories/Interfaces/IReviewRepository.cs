using BdmiAPI.Models;

namespace BdmiAPI.Repositories.Interfaces
{
    public interface IReviewRepository
    {
        IQueryable<Review> Query();
        Task<Review?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<bool> ExistsUserMovieAsync(int userId, int movieId, int? excludeId = null, CancellationToken ct = default);
        Task<bool> MovieExistsAsync(int movieId, CancellationToken ct = default);
        Task<bool> UserExistsAsync(int userId, CancellationToken ct = default);
        Task AddAsync(Review entity, CancellationToken ct = default);
        void Remove(Review entity);
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
