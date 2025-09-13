using BdmiAPI.Database;
using BdmiAPI.Repositories.Interfaces;
using BdmiAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BdmiAPI.Repositories
{

    public sealed class MovieRepository : IMovieRepository
    {
        private readonly AppDb _db;
        public MovieRepository(AppDb db) => _db = db;

        public IQueryable<Movie> Query() =>
            _db.Movies.AsNoTracking()
              .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre)
              .Include(m => m.Reviews); // for Average

        public Task<Movie?> GetByIdAsync(int id, CancellationToken ct = default) =>
            _db.Movies.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id, ct);

        public Task<Movie?> GetByIdWithGenresAsync(int id, CancellationToken ct = default) =>
            _db.Movies
              .Include(m => m.MovieGenres)
              .ThenInclude(mg => mg.Genre)
              .Include(m => m.Reviews)
              .FirstOrDefaultAsync(m => m.Id == id, ct);

        public Task<bool> ExistsTitleYearAsync(string title, int releaseYear, int? excludeId = null, CancellationToken ct = default)
        {
            var q = _db.Movies.AsQueryable().Where(m => m.Title == title && m.ReleaseYear == releaseYear);
            if (excludeId is not null) q = q.Where(m => m.Id != excludeId);
            return q.AnyAsync(ct);
        }

        public async Task<bool> GenresExistAsync(IEnumerable<int> genreIds, CancellationToken ct = default)
        {
            var set = genreIds?.Distinct().ToArray() ?? Array.Empty<int>();
            if (set.Length == 0) return true;
            var count = await _db.Genres.CountAsync(g => set.Contains(g.Id), ct);
            return count == set.Length;
        }

        public Task AddAsync(Movie entity, CancellationToken ct = default) =>
            _db.Movies.AddAsync(entity, ct).AsTask();

        public void Remove(Movie entity) => _db.Movies.Remove(entity);

        public Task<int> SaveChangesAsync(CancellationToken ct = default) =>
            _db.SaveChangesAsync(ct);
    }
}
