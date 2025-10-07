using BdmiAPI.Database;
using BdmiAPI.DTOs;
using BdmiAPI.Models;
using BdmiAPI.Repositories.Interfaces;
using BdmiAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BdmiAPI.Services
{
    public sealed class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly AppDb _db;

        private const int DeletedUserId = 1;

        public UserService(IUserRepository repo, AppDb db)
        {
            _repo = repo;
            _db = db;
        }

        public async Task<IReadOnlyList<UserListItemDto>> ListAsync(string? q, CancellationToken ct = default)
        {
            var query = _repo.Query();
            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(u => u.Username.Contains(q) || u.Email.Contains(q));

            var list = await query
                .OrderBy(u => u.Username)
                .Select(u => new UserListItemDto(u.Id, u.Username, u.Email, u.CreatedAt))
                .ToListAsync(ct);

            return list;
        }

        public async Task<UserDetailsDto?> GetAsync(int id, CancellationToken ct = default)
        {
            var u = await _repo.GetByIdAsync(id, ct);
            return u is null ? null : new UserDetailsDto(u.Id, u.Username, u.Email, u.CreatedAt);
        }

        public async Task<UserDetailsDto> CreateAsync(CreateUserDto dto, CancellationToken ct = default)
        {
            var username = dto.Username.Trim();
            var email = dto.Email.Trim();

            if (await _repo.ExistsByUsernameAsync(username, null, ct))
                throw new UserConflictException("Username already in use.");

            if (await _repo.ExistsByEmailAsync(email, null, ct))
                throw new UserConflictException("Email already in use.");

            var entity = new User { Username = username, Email = email };
            await _repo.AddAsync(entity, ct);
            await _repo.SaveChangesAsync(ct);

            return new UserDetailsDto(entity.Id, entity.Username, entity.Email, entity.CreatedAt);
        }

        public async Task<bool> UpdateAsync(int id, UpdateUserDto dto, CancellationToken ct = default)
        {
            var entity = await _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
            if (entity is null) return false;

            var username = dto.Username.Trim();
            var email = dto.Email.Trim();

            if (await _repo.ExistsByUsernameAsync(username, excludeId: id, ct))
                throw new UserConflictException("Username already in use.");

            if (await _repo.ExistsByEmailAsync(email, excludeId: id, ct))
                throw new UserConflictException("Email already in use.");

            entity.Username = username;
            entity.Email = email;

            await _repo.SaveChangesAsync(ct);
            return true;
        }

        public Task<bool> DeleteAsync(int id, CancellationToken ct = default)
            => AnonymizeAndDeleteAsync(id, ct);
    

        public async Task<bool> AnonymizeAndDeleteAsync(int id, CancellationToken ct = default)
        {
            var victim = await _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
            if (victim is null) return false;
            if (victim.Id == DeletedUserId || victim.IsSystem)
                throw new UserForbiddenOperationException("Cannot anonymize the system user.");

            var deletedUser = await _db.Users.FirstOrDefaultAsync(u => u.Id == DeletedUserId && u.IsSystem, ct);
            if (deletedUser is null)
                throw new InvalidOperationException("Deleted user is not seeded. Run migrations/seeding.");

            using var tx = await _db.Database.BeginTransactionAsync(ct);

            // Reassign ALL content to Deleted User
            await _db.Movies
                .Where(m => m.UploadedByUserId == id)
                .ExecuteUpdateAsync(up => up.SetProperty(m => m.UploadedByUserId, DeletedUserId), ct);

            await _db.Reviews
                .Where(r => r.UserId == id)
                .ExecuteUpdateAsync(up => up.SetProperty(r => r.UserId, DeletedUserId), ct);

            _db.Users.Remove(victim);
            await _db.SaveChangesAsync(ct);

            await tx.CommitAsync(ct);
            return true;
        }
    }
}
