using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tidbeat.Models {
    /// <summary>
    /// The post model.
    /// </summary>
    public class Post {
        /// <summary>
        /// The post id.
        /// </summary>
        [Key]
        public int PostId { get; set; }

        /// <summary>
        /// The post's title.
        /// </summary>
        [Required(ErrorMessage = "Por favor, adicione um título ao seu post.")]
        [DisplayName("Título")]
        public string Title { get; set; }

        /// <summary>
        /// The post's content.
        /// </summary>
        [Required(ErrorMessage = "Por favor, adicione texto ao conteúdo do seu post.")]
        [DisplayName("Conteúdo")]
        public string Content { get; set; }

        /// <summary>
        /// The post's associated song.
        /// </summary>
        public Song? Song { get; set; }

        /// <summary>
        /// The post's associated band.
        /// </summary>
        public Band? Band { get; set; }

        /// <summary>
        /// The post's owner.
        /// </summary>
        public ApplicationUser? User { get; set; }

        /// <summary>
        /// The post's creation date.
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// The post's edit date.
        /// </summary>
        public DateTime EditDate { get; set; }

        /// <summary>
        /// The post's boolean about whether the post has been edited before.
        /// </summary>
        public bool IsEdited { get; set; } = false;

    }
}
