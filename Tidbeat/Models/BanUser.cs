using System.ComponentModel.DataAnnotations;

namespace Tidbeat.Models
{
    public class BanUser
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public ApplicationUser User { get; set; }

        public string? Reason { get; set; }

        [Required]
        public DateTime EndsAt { get; set; }

    }
}
