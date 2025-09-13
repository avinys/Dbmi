using BdmiAPI.Models;

namespace BdmiAPI.Repositories.Interfaces;

public interface IMovieRepository
{
    IQueryable<Movie> Query();
    Task<Movie?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Movie?> GetByIdWithGenresAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsTitleYearAsync(string title, int releaseYear, int? excludeId = null, CancellationToken ct = default);
    Task<bool> GenresExistAsync(IEnumerable<int> genreIds, CancellationToken ct = default);
    Task AddAsync(Movie entity, CancellationToken ct = default);
    void Remove(Movie entity);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
