using System.ComponentModel.DataAnnotations;

namespace BdmiAPI.DTOs
{
    public record ReviewListItemDto(
        int Id,
        int MovieId,
        int UserId,
        int Score,
        string? Title,
        DateTime CreatedAt
    );

    public record ReviewDetailsDto(
        int Id,
        int MovieId,
        int UserId,
        int Score,
        string? Title,
        string? Body,
        DateTime CreatedAt
    );

    public sealed class CreateReviewDto
    {
        [Required] public int MovieId { get; set; }
        [Required] public int UserId { get; set; }
        [Range(1, 10)] public int Score { get; set; }
        [StringLength(120)] public string? Title { get; set; }
        [StringLength(4000)] public string? Body { get; set; }
    }

    public sealed class UpdateReviewDto
    {
        [Range(1, 10)] public int Score { get; set; }
        [StringLength(120)] public string? Title { get; set; }
        [StringLength(4000)] public string? Body { get; set; }
    }
}
