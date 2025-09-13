using System.ComponentModel.DataAnnotations;

namespace BdmiAPI.Models
{
    public class Review : BaseEntity
    {
        public int MovieId { get; set; }
        public Movie Movie { get; set; } = default!;

        public int UserId { get; set; }
        public User User { get; set; } = default!;

        [Required, StringLength(120)]
        public string Title { get; set; } = default!;

        [Required, StringLength(4000)]
        public string Body { get; set; } = default!;

        [Range(1, 10)]
        public int Score { get; set; }

        public bool IsHidden { get; set; } = false;
    }
}
