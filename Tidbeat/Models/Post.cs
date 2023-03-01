using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tidbeat.Models {
    public class Post {
        [Key]
        public int PostId { get; set; }

        [Required]
        [DisplayName("Título")]
        public string Title { get; set; }

        [Required]
        [DisplayName("Conteúdo")]
        public string Content { get; set; }

        public Song? Song { get; set; }

        public Band? Band { get; set; }

        [Required]
        public ApplicationUser User { get; set; }
    }
}
