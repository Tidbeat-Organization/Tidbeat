using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tidbeat.Models
{
    /// <summary>
    /// The comment model.
    /// </summary>
    public class Comment
    {
        /// <summary>
        /// The comment id.
        /// </summary>
        [Key]
        public int CommentId { get; set; }

        /// <summary>
        /// The comment's post.
        /// </summary>
        public Post? post { get; set; }

        /// <summary>
        /// The comment's content.
        /// </summary>
        [DisplayName("Conteúdo")]
        public string Content { get; set; }

        /// <summary>
        /// The comment's owner.
        /// </summary>
        public ApplicationUser? User { get; set; }

        /// <summary>
        /// The comment's creation date.
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// The comment's edit date.
        /// </summary>
        public DateTime EditDate { get; set; }

        /// <summary>
        /// Checks if the comments was edited.
        /// </summary>
        public bool IsEdited { get; set; } = false;
    }
}
