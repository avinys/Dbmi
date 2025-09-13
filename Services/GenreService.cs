using BdmiAPI.Models;
using BdmiAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BdmiAPI.Application.Genres;

public sealed class GenreService : IGenreService
{
    private readonly IGenreRepository _repo;
    public GenreService(IGenreRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<GenreListItemDto>> ListAsync(string? q, CancellationToken ct = default)
    {
        var query = _repo.Query();
        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(g => g.Name.Contains(q));
        var items = await query.OrderBy(g => g.Name).ToListAsync(ct);
        return items.Select(g => g.ToListItem()).ToList();
    }

    public async Task<GenreDetailsDto?> GetAsync(int id, CancellationToken ct = default)
        => (await _repo.GetByIdAsync(id, ct))?.ToDetails();

    public async Task<GenreDetailsDto> CreateAsync(CreateGenreDto dto, CancellationToken ct = default)
    {
        if (await _repo.ExistsByNameAsync(dto.Name, null, ct))
            throw new GenreConflictException($"Genre '{dto.Name}' already exists.");

        var g = new Genre { Name = dto.Name.Trim() };
        await _repo.AddAsync(g, ct);
        await _repo.SaveChangesAsync(ct);
        return g.ToDetails();
    }

    public async Task<bool> UpdateAsync(int id, UpdateGenreDto dto, CancellationToken ct = default)
    {
        var g = await _repo.GetByIdAsync(id, ct);
        if (g is null) return false;

        if (await _repo.ExistsByNameAsync(dto.Name, excludeId: id, ct))
            throw new GenreConflictException($"Genre '{dto.Name}' already exists.");

        g.Name = dto.Name.Trim();
        await _repo.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var g = await _repo.GetByIdAsync(id, ct);
        if (g is null) return false;

        _repo.Remove(g);
        await _repo.SaveChangesAsync(ct);
        return true;
    }
}

public sealed class GenreConflictException : Exception
{
    public GenreConflictException(string message) : base(message) { }
}
