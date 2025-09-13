using BdmiAPI.Database;
using BdmiAPI.DTOs;
using BdmiAPI.Models;
using BdmiAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BdmiAPI.Services.Interfaces
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

        public async Task<IReadOnlyList<MovieListItemDto>> ListAsync(int? genreId, string? q, string? sort, CancellationToken ct = default)
        {
            var query = _repo.Query();

            if (genreId is not null)
                query = query.Where(m => m.MovieGenres.Any(g => g.GenreId == genreId));

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(m => m.Title.Contains(q) || m.Description.Contains(q));

            // compute average on the fly
            var projected = query.Select(m => new MovieListItemDto(
                m.Id,
                m.Title,
                m.ReleaseYear,
                m.DurationMin,
                m.MovieGenres.Select(mg => mg.Genre.Name).OrderBy(n => n).ToList(),
                m.Reviews.Any() ? m.Reviews.Average(r => r.Score) : 0
            ));

            projected = sort switch
            {
                "rating" => projected.OrderByDescending(x => x.AverageScore).ThenBy(x => x.Title),
                "date" => projected.OrderByDescending(x => x.ReleaseYear).ThenBy(x => x.Title),
                _ => projected.OrderBy(x => x.Title)
            };

            return await projected.ToListAsync(ct);
        }

        public async Task<MovieDetailsDto?> GetAsync(int id, CancellationToken ct = default)
        {
            var m = await _repo.GetByIdWithGenresAsync(id, ct);
            if (m is null) return null;

            return new MovieDetailsDto(
                m.Id, m.Title, m.Description, m.ReleaseYear, m.DurationMin, m.UploadedByUserId,
                m.MovieGenres.Select(mg => mg.GenreId).ToList(),
                m.Reviews.Any() ? m.Reviews.Average(r => r.Score) : 0
            );
        }

        public async Task<MovieDetailsDto> CreateAsync(CreateMovieDto dto, CancellationToken ct = default)
        {
            if (!await _repo.GenresExistAsync(dto.GenreIds, ct))
                throw new MovieValidationException("One or more genres do not exist.");

            if (await _repo.ExistsTitleYearAsync(dto.Title.Trim(), dto.ReleaseYear, null, ct))
                throw new MovieConflictException($"Movie '{dto.Title}' ({dto.ReleaseYear}) already exists.");

            var m = new Movie
            {
                Title = dto.Title.Trim(),
                Description = dto.Description.Trim(),
                ReleaseYear = dto.ReleaseYear,
                DurationMin = dto.DurationMin,
                UploadedByUserId = dto.UploadedByUserId,
                MovieGenres = dto.GenreIds.Distinct().Select(gid => new MovieGenre { GenreId = gid }).ToList()
            };

            await _repo.AddAsync(m, ct);
            await _repo.SaveChangesAsync(ct);

            return (await GetAsync(m.Id, ct))!;
        }

        public async Task<bool> UpdateAsync(int id, UpdateMovieDto dto, CancellationToken ct = default)
        {
            if (!await _repo.GenresExistAsync(dto.GenreIds, ct))
                throw new MovieValidationException("One or more genres do not exist.");

            if (await _repo.ExistsTitleYearAsync(dto.Title.Trim(), dto.ReleaseYear, id, ct))
                throw new MovieConflictException($"Movie '{dto.Title}' ({dto.ReleaseYear}) already exists.");

            var m = await _db.Movies.Include(x => x.MovieGenres).FirstOrDefaultAsync(x => x.Id == id, ct);
            if (m is null) return false;

            m.Title = dto.Title.Trim();
            m.Description = dto.Description.Trim();
            m.ReleaseYear = dto.ReleaseYear;
            m.DurationMin = dto.DurationMin;

            m.MovieGenres.Clear();
            foreach (var gid in dto.GenreIds.Distinct())
                m.MovieGenres.Add(new MovieGenre { MovieId = id, GenreId = gid });

            await _repo.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var m = await _db.Movies.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (m is null) return false;

            _repo.Remove(m);
            await _repo.SaveChangesAsync(ct);
            return true;
        }

        public async Task<object?> GetReviewsForMovieAsync(int id, bool includeText, CancellationToken ct = default)
        {
            var exists = await _db.Movies.AnyAsync(m => m.Id == id, ct);
            if (!exists) return null;

            var reviews = await _db.Reviews
                .Where(r => r.MovieId == id)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReviewForMovieDto
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    Score = r.Score,
                    Title = includeText ? r.Title : null,
                    Body = includeText ? r.Body : null,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync(ct);

            return new MovieReviewsDto { MovieId = id, Reviews = reviews };
        }
    }

}

