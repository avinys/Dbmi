using BdmiAPI.Database;
using BdmiAPI.Models;
using BdmiAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BdmiAPI.Repositories
{
    public sealed class ReviewRepository : IReviewRepository
    {
        private readonly AppDb _db;
        public ReviewRepository(AppDb db) => _db = db;

        public IQueryable<Review> Query() =>
            _db.Reviews.AsNoTracking();

        public Task<Review?> GetByIdAsync(int id, CancellationToken ct = default) =>
            _db.Reviews.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id, ct);

        public Task<bool> ExistsUserMovieAsync(int userId, int movieId, int? excludeId = null, CancellationToken ct = default)
        {
            var q = _db.Reviews.AsQueryable().Where(r => r.UserId == userId && r.MovieId == movieId);
            if (excludeId is not null) q = q.Where(r => r.Id != excludeId);
            return q.AnyAsync(ct);
        }

        public Task<bool> MovieExistsAsync(int movieId, CancellationToken ct = default) =>
            _db.Movies.AnyAsync(m => m.Id == movieId, ct);

        public Task<bool> UserExistsAsync(int userId, CancellationToken ct = default) =>
            _db.Users.AnyAsync(u => u.Id == userId, ct);

        public Task AddAsync(Review entity, CancellationToken ct = default) =>
            _db.Reviews.AddAsync(entity, ct).AsTask();

        public void Remove(Review entity) => _db.Reviews.Remove(entity);

        public Task<int> SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
    }
}
