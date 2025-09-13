using BdmiAPI.Models;

namespace BdmiAPI.Repositories.Interfaces
{
    public interface IGenreRepository
    {
        IQueryable<Genre> Query();
        Task<Genre?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken ct = default);
        Task AddAsync(Genre entity, CancellationToken ct = default);
        void Remove(Genre entity);
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}