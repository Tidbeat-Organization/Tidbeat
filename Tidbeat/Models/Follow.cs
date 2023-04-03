using System.ComponentModel.DataAnnotations;

namespace Tidbeat.Models
{
    public class Follow
    {
        [Key]
        public int Id { get; set; }
        public ApplicationUser UserAsker { get; set; }
        public ApplicationUser UserFollowed { get; set; }
    }
}
