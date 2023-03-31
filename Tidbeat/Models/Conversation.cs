using System.ComponentModel.DataAnnotations;

namespace Tidbeat.Models {
    public class Conversation {
        [Key]
        public string Id { get; set; }
        public string? Title { get; set; }
        public DateTime StartDate { get; set; }
        public bool IsGroupConversation { get; set; }
    }
}
