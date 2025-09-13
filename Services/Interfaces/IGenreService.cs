using BdmiAPI.DTOs;

namespace BdmiAPI.Services.Interfaces
{
    public interface IGenreService
    {
        Task<IReadOnlyList<GenreListItemDto>> ListAsync(string? q, CancellationToken ct = default);
        Task<GenreDetailsDto?> GetAsync(int id, CancellationToken ct = default);
        Task<GenreDetailsDto> CreateAsync(CreateGenreDto dto, CancellationToken ct = default);
        Task<bool> UpdateAsync(int id, UpdateGenreDto dto, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
