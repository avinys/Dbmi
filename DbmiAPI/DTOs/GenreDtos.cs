using System.ComponentModel.DataAnnotations;
using BdmiAPI.Models;

namespace BdmiAPI.DTOs
{
    public record GenreListItemDto(int Id, string Name);
    public record GenreDetailsDto(int Id, string Name);

    public class CreateGenreDto
    {
        [Required, StringLength(50)]
        public string Name { get; set; } = default!;

        [StringLength(500)]
        public string? Description { get; set; }
    }

    public class UpdateGenreDto
    {
        [Required, StringLength(50)]
        public string Name { get; set; } = default!;

        [StringLength(500)]
        public string? Description { get; set; }
    }

    internal static class GenreMapping
    {
        public static GenreListItemDto ToListItem(this Genre g) => new(g.Id, g.Name);
        public static GenreDetailsDto ToDetails(this Genre g) => new(g.Id, g.Name);
    }

}