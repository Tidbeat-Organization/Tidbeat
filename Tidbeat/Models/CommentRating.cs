using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tidbeat.Models {
    /// <summary>
    /// The comment rating model.
    /// </summary>
    public class CommentRating : IRating {
        /// <summary>
        /// The comment rating id.
        /// </summary>
        [Key]
        public int RatingId { get; set; }

        /// <summary>
        /// The comment rating's value.
        /// </summary>
        [Range(0, 5, ErrorMessage = "Value must be between 0 and 5")]
        public int Value { get; set; }

        /// <summary>
        /// The comment rating's owner.
        /// </summary>
        [Required]
        public ApplicationUser User { get; set; }

        /// <summary>
        /// The comment which the rating belongs to.
        /// </summary>
        [ForeignKey("CommentId")]
        public Comment? Comment { get; set; }
    }
}
