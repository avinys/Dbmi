using BdmiAPI.DTOs;
using BdmiAPI.Models;
using BdmiAPI.Repositories.Interfaces;
using BdmiAPI.Services.Authorization;
using BdmiAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BdmiAPI.Services
{
    public sealed class ReviewService : IReviewService
    {
        private readonly IReviewRepository _repo;
        public ReviewService(IReviewRepository repo) => _repo = repo;

        public async Task<IReadOnlyList<ReviewListItemDto>> ListAsync(int? movieId, int? userId, ClaimsPrincipal user, CancellationToken ct = default)
        {
            var q = _repo.Query();

            if (movieId is not null) q = q.Where(r => r.MovieId == movieId);
            
            // Users can only see their own reviews unless they're admin
            if (!AuthorizationHelper.IsAdmin(user))
            {
                var currentUserId = AuthorizationHelper.GetUserId(user);
                if (userId is not null && userId != currentUserId)
                    throw new UnauthorizedAccessException("You can only view your own reviews.");
                
                q = q.Where(r => r.UserId == currentUserId);
            }
            else if (userId is not null)
            {
                q = q.Where(r => r.UserId == userId);
            }

            var list = await q
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReviewListItemDto(r.Id, r.MovieId, r.UserId, r.Score, r.Title, r.CreatedAt))
                .ToListAsync(ct);

            return list;
        }

        public async Task<ReviewDetailsDto?> GetAsync(int id, ClaimsPrincipal user, CancellationToken ct = default)
        {
            var review = await _repo.GetByIdAsync(id, ct);
            if (review is null) return null;

            // Verify access
            if (!AuthorizationHelper.IsAdmin(user) && review.UserId != AuthorizationHelper.GetUserId(user))
                throw new UnauthorizedAccessException("You can only view your own reviews.");

            return new ReviewDetailsDto(review.Id, review.MovieId, review.UserId, review.Score, 
                                     review.Title, review.Body, review.CreatedAt);
        }

        public async Task<ReviewDetailsDto> CreateAsync(CreateReviewDto dto, ClaimsPrincipal user, CancellationToken ct = default)
        {
            if (!AuthorizationHelper.IsAuthenticated(user))
                throw new UnauthorizedAccessException("You must be logged in to create reviews.");

            var userId = AuthorizationHelper.GetUserId(user) ?? 
                throw new UnauthorizedAccessException("Invalid user claims.");

            // Only admins can create reviews for other users
            if (dto.UserId != userId && !AuthorizationHelper.IsAdmin(user))
                throw new UnauthorizedAccessException("You can only create reviews for yourself.");

            // Validate FK existence
            if (!await _repo.MovieExistsAsync(dto.MovieId, ct))
                throw new ReviewValidationException("Movie does not exist.");
            if (!await _repo.UserExistsAsync(dto.UserId, ct))
                throw new ReviewValidationException("User does not exist.");

            // Unique (UserId, MovieId)
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

        public async Task<bool> UpdateAsync(int id, UpdateReviewDto dto, ClaimsPrincipal user, CancellationToken ct = default)
        {
            var entity = await _repo.Query().FirstOrDefaultAsync(r => r.Id == id, ct);
            if (entity is null) return false;

            // Verify ownership or admin status
            if (!AuthorizationHelper.CanModifyResource(user, entity.UserId))
                throw new UnauthorizedAccessException("You can only update your own reviews.");

            entity.Score = dto.Score;
            entity.Title = dto.Title?.Trim();
            entity.Body = dto.Body?.Trim();

            await _repo.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, ClaimsPrincipal user, CancellationToken ct = default)
        {
            var entity = await _repo.Query().FirstOrDefaultAsync(r => r.Id == id, ct);
            if (entity is null) return false;

            // Verify ownership or admin status
            if (!AuthorizationHelper.CanModifyResource(user, entity.UserId))
                throw new UnauthorizedAccessException("You can only delete your own reviews.");

            _repo.Remove(entity);
            await _repo.SaveChangesAsync(ct);
            return true;
        }
    }
}
