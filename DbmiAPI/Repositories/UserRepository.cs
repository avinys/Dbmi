using BdmiAPI.Database;
using BdmiAPI.Models;
using BdmiAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BdmiAPI.Repositories
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly AppDb _db;
        public UserRepository(AppDb db) => _db = db;

        public IQueryable<User> Query() => _db.Users.AsNoTracking();

        public Task<User?> GetByIdAsync(int id, CancellationToken ct = default) =>
            _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);

        public Task<bool> ExistsByEmailAsync(string email, int? excludeId = null, CancellationToken ct = default)
        {
            var q = _db.Users.AsQueryable().Where(u => u.Email == email);
            if (excludeId is not null) q = q.Where(u => u.Id != excludeId);
            return q.AnyAsync(ct);
        }

        public Task<bool> ExistsByUsernameAsync(string username, int? excludeId = null, CancellationToken ct = default)
        {
            var q = _db.Users.AsQueryable().Where(u => u.Username == username);
            if (excludeId is not null) q = q.Where(u => u.Id != excludeId);
            return q.AnyAsync(ct);
        }

        public Task AddAsync(User entity, CancellationToken ct = default) =>
            _db.Users.AddAsync(entity, ct).AsTask();

        public void Remove(User entity) => _db.Users.Remove(entity);

        public Task<int> SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
    }
}
