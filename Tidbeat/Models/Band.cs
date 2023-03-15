using System.ComponentModel.DataAnnotations;

namespace Tidbeat.Models {
    public class Band {
        [Key]
        public string BandId { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Image { get; set; }
    }
}
