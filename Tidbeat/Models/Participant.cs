namespace Tidbeat.Models {
    public class Participant {
        public int Id { get; set; }
        public Conversation Conversation { get; set; }
        public ApplicationUser User { get; set; }
    }
}
