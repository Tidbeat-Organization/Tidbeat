using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tidbeat.Models {
    public class Post {
        [Key]
        public int PostId { get; set; }

        [Required(ErrorMessage = "Por favor, adicione um título ao seu post.")]
        [DisplayName("Título")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Por favor, adicione texto ao conteúdo do seu post.")]
        [DisplayName("Conteúdo")]
        public string Content { get; set; }

        public Song? Song { get; set; }

        public Band? Band { get; set; }

        public ApplicationUser? User { get; set; }

        public IEnumerable<Comment>? Comments { get; set; }
    }
}
