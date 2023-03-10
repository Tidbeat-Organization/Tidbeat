using System.ComponentModel;

namespace Tidbeat.Models
{
    public class Comment
    {
        //[Key]
        public int CommentId { get; set; }

        public Post post { get; set; }

        //[Required(ErrorMessage = "Por favor, adicione texto ao conteúdo do seu post.")]
        [DisplayName("Conteúdo")]
        public string Content { get; set; }

        public ApplicationUser? User { get; set; }
    }
}
