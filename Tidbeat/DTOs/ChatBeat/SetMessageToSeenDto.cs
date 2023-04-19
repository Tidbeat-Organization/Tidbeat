namespace Tidbeat.DTOs.ChatBeat {
    /// <summary>
    /// The DTO for setting all messages of a user in a conversation to seen.
    /// </summary>
    public class SetMessageToSeenDto {
        /// <summary>
        /// The id of the message.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// The id of the conversation.
        /// </summary>
        public string ConversationId { get; set; }
    }
}
