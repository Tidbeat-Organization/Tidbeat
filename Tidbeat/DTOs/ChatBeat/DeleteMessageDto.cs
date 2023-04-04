namespace Tidbeat.DTOs.ChatBeat {
    /// <summary>
    /// The DTO for deleting a message.
    /// </summary>
    public class DeleteMessageDto {
        /// <summary>
        /// The id of the message.
        /// </summary>
        public int MessageId { get; set; }

        /// <summary>
        /// The id of the user.
        /// </summary>
        public string UserId { get; set; }
    }
}
