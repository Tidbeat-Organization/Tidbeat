namespace Tidbeat.DTOs.ChatBeat {
    /// <summary>
    /// The DTO for editing a message.
    /// </summary>
    public class EditMessageDto {
        /// <summary>
        /// The id of the message.
        /// </summary>
        public int MessageId { get; set; }

        /// <summary>
        /// The new text of the message.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The id of the user.
        /// </summary>
        public string UserId { get; set; }
    }
}
