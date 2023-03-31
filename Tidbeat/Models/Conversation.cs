namespace Tidbeat.Models {
    public class Conversation {
        public int Id { get; set; }
        public string? Title { get; set; }
        public DateTime StartDate { get; set; }
        public bool IsGroupConversation { get; set; }
    }
}
