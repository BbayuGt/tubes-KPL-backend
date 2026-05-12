using System.ComponentModel.DataAnnotations;

namespace tubes_KPL_backend.Models
{
    public class Campaign
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public decimal TargetAmount { get; set; }

        public decimal CollectedAmount { get; set; } = 0;

        public string ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}