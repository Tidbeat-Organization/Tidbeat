namespace Tidbeat.Models {
    public class Message {
        public int Id { get; set; }
        public string Text { get; set; }
        public int Status { get; set; }
        public DateTime Created { get; set; }
        public ApplicationUser User { get; set; }
        public Conversation Conversation { get; set; }
    }
}
