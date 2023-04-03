namespace Tidbeat.Models
{
    public class Follow
    {
        public int Id { get; set; }
        public ApplicationUser UserAsker { get; set; }
        public ApplicationUser UserFollowed { get; set; }
    }
}
