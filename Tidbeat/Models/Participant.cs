namespace Tidbeat.Models {
    /// <summary>
    /// The participant of a conversation. This was added because of the future possibility of adding group conversations.
    /// </summary>
    public class Participant {
        /// <summary>
        /// The participation id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The conversation id.
        /// </summary>
        public Conversation Conversation { get; set; }

        /// <summary>
        /// The user which is the participant of the conversation.
        /// </summary>
        public ApplicationUser User { get; set; }
    }
}
