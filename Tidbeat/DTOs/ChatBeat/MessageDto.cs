namespace Tidbeat.DTOs.ChatBeat
{
    /// <summary>
    /// The DTO for sending a message.
    /// </summary>
    public class MessageDto
    {
        /// <summary>
        /// The text of the message.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The id of the user.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// The id of the conversation.
        /// </summary>
        public string ConversationId { get; set; }
    }
}
