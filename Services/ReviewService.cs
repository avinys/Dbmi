using BdmiAPI.DTOs;
using BdmiAPI.Models;
using BdmiAPI.Repositories.Interfaces;
using BdmiAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BdmiAPI.Services
{
    public sealed class ReviewService : IReviewService
    {
        private readonly IReviewRepository _repo;
        public ReviewService(IReviewRepository repo) => _repo = repo;

        public async Task<IReadOnlyList<ReviewListItemDto>> ListAsync(int? movieId, int? userId, CancellationToken ct = default)
        {
            var q = _repo.Query();

            if (movieId is not null) q = q.Where(r => r.MovieId == movieId);
            if (userId is not null) q = q.Where(r => r.UserId == userId);

            var list = await q
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReviewListItemDto(r.Id, r.MovieId, r.UserId, r.Score, r.Title, r.CreatedAt))
                .ToListAsync(ct);

            return list;
        }

        public async Task<ReviewDetailsDto?> GetAsync(int id, CancellationToken ct = default)
        {
            var r = await _repo.GetByIdAsync(id, ct);
            return r is null ? null
                : new ReviewDetailsDto(r.Id, r.MovieId, r.UserId, r.Score, r.Title, r.Body, r.CreatedAt);
        }

        public async Task<ReviewDetailsDto> CreateAsync(CreateReviewDto dto, CancellationToken ct = default)
        {
            // validate FK existence
            if (!await _repo.MovieExistsAsync(dto.MovieId, ct))
                throw new ReviewValidationException("Movie does not exist.");
            if (!await _repo.UserExistsAsync(dto.UserId, ct))
                throw new ReviewValidationException("User does not exist.");

            // unique (UserId, MovieId)
            if (await _repo.ExistsUserMovieAsync(dto.UserId, dto.MovieId, null, ct))
                throw new ReviewConflictException("User has already reviewed this movie.");

            var entity = new Review
            {
                MovieId = dto.MovieId,
                UserId = dto.UserId,
                Score = dto.Score,
                Title = dto.Title?.Trim(),
                Body = dto.Body?.Trim()
            };

            await _repo.AddAsync(entity, ct);
            await _repo.SaveChangesAsync(ct);

            return new ReviewDetailsDto(entity.Id, entity.MovieId, entity.UserId, entity.Score,
                                        entity.Title, entity.Body, entity.CreatedAt);
        }

        public async Task<bool> UpdateAsync(int id, UpdateReviewDto dto, CancellationToken ct = default)
        {
            // load tracked entity (no dedicated repo method needed for a simple update)
            var entity = await _repo.Query().Where(r => r.Id == id).FirstOrDefaultAsync(ct);
            if (entity is null) return false;

            entity.Score = dto.Score;
            entity.Title = dto.Title?.Trim();
            entity.Body = dto.Body?.Trim();

            await _repo.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await _repo.Query().Where(r => r.Id == id).FirstOrDefaultAsync(ct);
            if (entity is null) return false;

            _repo.Remove(entity);
            await _repo.SaveChangesAsync(ct);
            return true;
        }
    }
}
