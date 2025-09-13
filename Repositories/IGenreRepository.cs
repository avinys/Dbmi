using BdmiAPI.Database;
using BdmiAPI.Models;
using BdmiAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BdmiAPI.Repositories
{
    public sealed class GenreRepository : IGenreRepository
    {
        private readonly AppDb _db;
        public GenreRepository(AppDb db) => _db = db;

        public IQueryable<Genre> Query() => _db.Genres.AsNoTracking();

        public Task<Genre?> GetByIdAsync(int id, CancellationToken ct = default) =>
            _db.Genres.FirstOrDefaultAsync(g => g.Id == id, ct);

        public Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken ct = default)
        {
            var q = _db.Genres.AsQueryable().Where(g => g.Name == name);
            if (excludeId is not null) q = q.Where(g => g.Id != excludeId);
            return q.AnyAsync(ct);
        }

        public Task AddAsync(Genre entity, CancellationToken ct = default) =>
            _db.Genres.AddAsync(entity, ct).AsTask();

        public void Remove(Genre entity) => _db.Genres.Remove(entity);

        public Task<int> SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
    }
}
