using System.Security.Claims;
using BdmiAPI.Database;
using BdmiAPI.DTOs;
using BdmiAPI.Models;
using BdmiAPI.Repositories.Interfaces;
using BdmiAPI.Services.Authorization;
using BdmiAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing.Internal;

namespace BdmiAPI.Services
{
    public sealed class MovieService : IMovieService
    {
        private readonly IMovieRepository _repo;
        private readonly AppDb _db;

        public MovieService(IMovieRepository repo, AppDb db)
        {
            _repo = repo;
            _db = db;
        }

        public async Task<IReadOnlyList<MovieListItemDto>> ListAsync(int? genreId, string? q, ClaimsPrincipal user, CancellationToken ct = default)
        {
            var query = _repo.Query();
            
            // Non-admin users can only see approved movies
            if (!AuthorizationHelper.IsAdmin(user))
                query = query.Where(m => m.Status == MovieStatus.Approved);
                
            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(m => m.Title.Contains(q));

            if(genreId != null)
                query = query.Where(m => m.MovieGenres.Any(mg => mg.GenreId == genreId));


            return await query
                .OrderByDescending(m => m.CreatedAt)
                .Select(m => new MovieListItemDto(
                    m.Id,
                    m.Title,
                    m.ReleaseYear,
                    m.DurationMin,
                    m.MovieGenres.Select(mg => mg.Genre.Name).OrderBy(n => n).ToList(),
                    m.Reviews.Any() ? m.Reviews.Average(r => r.Score) : 0
                ))
                .ToListAsync(ct);
        }

        public async Task<MovieDetailsDto?> GetAsync(int id, ClaimsPrincipal user, CancellationToken ct = default)
        {
            var movie = await _repo.GetByIdAsync(id, ct);
            if (movie == null) return null;

            // Non-admin users can only view approved movies
            if (!AuthorizationHelper.IsAdmin(user) && movie.Status != MovieStatus.Approved)
                throw new UnauthorizedAccessException("You don't have permission to view this movie.");

            return new MovieDetailsDto(
                movie.Id, movie.Title, movie.Description, movie.ReleaseYear, movie.DurationMin, movie.UploadedByUserId,
                movie.MovieGenres.Select(mg => mg.GenreId).ToList(),
                movie.Reviews.Any() ? movie.Reviews.Average(r => r.Score) : 0
            );
        }

        public async Task<MovieDetailsDto> CreateAsync(CreateMovieDto dto, ClaimsPrincipal user, CancellationToken ct = default)
        {
            if (!AuthorizationHelper.IsAuthenticated(user))
                throw new UnauthorizedAccessException("You must be logged in to create a movie.");

            var userId = AuthorizationHelper.GetUserId(user) ?? 
                throw new UnauthorizedAccessException("Invalid user claims.");

            var entity = new Movie
            {
                Title = dto.Title.Trim(),
                Description = dto.Description?.Trim(),
                ReleaseYear = dto.ReleaseYear,
                DurationMin = dto.DurationMin,
                Status = AuthorizationHelper.IsAdmin(user) ? MovieStatus.Approved : MovieStatus.Pending,
                UploadedByUserId = userId,
                MovieGenres = dto.GenreIds.Select(gid => new MovieGenre { GenreId = gid }).ToList()
            };

            await _repo.AddAsync(entity, ct);
            await _repo.SaveChangesAsync(ct);

            return new MovieDetailsDto(
                entity.Id, entity.Title, entity.Description, entity.ReleaseYear, entity.DurationMin, entity.UploadedByUserId,
                entity.MovieGenres.Select(mg => mg.GenreId).ToList(),
                entity.Reviews.Any() ? entity.Reviews.Average(r => r.Score) : 0
            );
        }

        public async Task<bool> UpdateAsync(int id, UpdateMovieDto dto, ClaimsPrincipal user, CancellationToken ct = default)
        {
            var movie = await _repo.GetByIdAsync(id, ct);
            if (movie == null) return false;

            // Only admin or the uploader can modify
            if (!AuthorizationHelper.CanModifyResource(user, movie.UploadedByUserId))
                throw new UnauthorizedAccessException("You don't have permission to update this movie.");

            movie.Title = dto.Title.Trim();
            movie.Description = dto.Description?.Trim();
            movie.ReleaseYear = dto.ReleaseYear;
            movie.DurationMin = dto.DurationMin;

            // If non-admin updates, set status back to pending
            if (!AuthorizationHelper.IsAdmin(user))
                movie.Status = MovieStatus.Pending;

            await _repo.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, ClaimsPrincipal user, CancellationToken ct = default)
        {
            var movie = await _repo.GetByIdAsync(id, ct);
            if (movie == null) return false;

            // Only admin or the uploader can delete
            if (!AuthorizationHelper.CanModifyResource(user, movie.UploadedByUserId))
                throw new UnauthorizedAccessException("You don't have permission to delete this movie.");

            _repo.Remove(movie);
            await _repo.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> ApproveAsync(int id, ClaimsPrincipal user, CancellationToken ct = default)
        {
            if (!AuthorizationHelper.IsAdmin(user))
                throw new UnauthorizedAccessException("Only administrators can approve movies.");

            var movie = await _repo.GetByIdAsync(id, ct);
            if (movie == null) return false;

            movie.Status = MovieStatus.Approved;
            await _repo.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> RejectAsync(int id, ClaimsPrincipal user, CancellationToken ct = default)
        {
            if (!AuthorizationHelper.IsAdmin(user))
                throw new UnauthorizedAccessException("Only administrators can reject movies.");

            var movie = await _repo.GetByIdAsync(id, ct);
            if (movie == null) return false;

            movie.Status = MovieStatus.Rejected;
            await _repo.SaveChangesAsync(ct);
            return true;
        }
    }

}

