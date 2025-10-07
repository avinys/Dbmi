using BdmiAPI.Models;

namespace BdmiAPI.Repositories.Interfaces
{
    public interface IUserRepository
    {
        IQueryable<User> Query();
        Task<User?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<bool> ExistsByEmailAsync(string email, int? excludeId = null, CancellationToken ct = default);
        Task<bool> ExistsByUsernameAsync(string username, int? excludeId = null, CancellationToken ct = default);
        Task AddAsync(User entity, CancellationToken ct = default);
        void Remove(User entity);
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
