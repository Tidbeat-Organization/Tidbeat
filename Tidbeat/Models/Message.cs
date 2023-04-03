namespace Tidbeat.Models {
    /// <summary>
    /// A message that is sent between users.
    /// </summary>
    public class Message {
        /// <summary>
        /// The id of the message.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The text of the message.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The status of the message. They are referred through the MessageStatus enum.
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// The date the message was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// The message's owner.
        /// </summary>
        public ApplicationUser User { get; set; }

        /// <summary>
        /// The conversation the message belongs to.
        /// </summary>
        public Conversation Conversation { get; set; }
    }
}
