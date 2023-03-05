using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tidbeat.Models {
    public class PostRating : IRating {
        [Key]
        public int RatingId { get; set; }
        [Range(1, 5, ErrorMessage = "Value must be between 1 and 5")]
        public int Value { get; set; }

        [Required]
        public ApplicationUser User { get; set; }

        [ForeignKey("PostId")]
        public Post? Post { get; set; }
    }
}
