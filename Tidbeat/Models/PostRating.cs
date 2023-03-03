using System.ComponentModel.DataAnnotations;

namespace Tidbeat.Models {
    public class PostRating {
        [Key]
        public int PostRatingId { get; set; }
        public int Value { get; set; }

        [Required]
        public ApplicationUser User { get; set; }

        [Required]
        public Post Post { get; set; }
    }
}
