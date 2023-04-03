using System.ComponentModel.DataAnnotations;

namespace Tidbeat.Models {
    /// <summary>
    /// The conversation model.
    /// </summary>
    public class Conversation {
        /// <summary>
        /// The conversation id.
        /// </summary>
        [Key]
        public string Id { get; set; }

        /// <summary>
        /// The conversation's title.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// The conversation's date which it started.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Checks if its a group conversation. IMPORTANT: UNIMPLEMENTED FUNCTIONALITY, SO ITS ALWAYS FALSE. IT WAS ONLY ADDED HERE IN CASE ITS NEEDED TO IMPLEMENT GROUP CONVERSATIONS.
        /// </summary>
        public bool IsGroupConversation { get; set; }
    }
}
