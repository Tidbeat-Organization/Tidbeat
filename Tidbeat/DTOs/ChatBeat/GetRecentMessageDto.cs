namespace Tidbeat.DTOs.ChatBeat {
    /// <summary>
    /// The DTO for getting recent messages.
    /// </summary>
    public class GetRecentMessageDto {
        /// <summary>
        /// The id of the conversation.
        /// </summary>
        public string ConversationId { get; set; }

        /// <summary>
        /// The amount of messages to get.
        /// </summary>
        public int MessageAmount { get; set; }

        /// <summary>
        /// The amount of messages to skip.
        /// </summary>
        public int SkipAmount { get; set; }
    }
}
