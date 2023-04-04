using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tidbeat.Models {
    /// <summary>
    /// The post rating model.
    /// </summary>
    public class PostRating : IRating {
        /// <summary>
        /// The post rating id.
        /// </summary>
        [Key]
        public int RatingId { get; set; }

        /// <summary>
        /// The post rating's value.
        /// </summary>
        [Range(0, 5, ErrorMessage = "Value must be between 0 and 5")]
        public int Value { get; set; }

        /// <summary>
        /// The owner of the rating.
        /// </summary>
        [Required]
        public ApplicationUser User { get; set; }

        /// <summary>
        /// The post id to which the rating is associated.
        /// </summary>
        [ForeignKey("PostId")]
        public Post? Post { get; set; }
    }
}
