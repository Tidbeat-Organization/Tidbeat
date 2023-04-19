namespace Tidbeat.Enums {
    /// <summary>
    /// The status of a message. It can be either Sent or Seen.
    /// </summary>
    public enum MessageStatus {
        /// <summary>
        /// The message has been sent, but not seen by the recipient.
        /// </summary>
        Sent, 
        /// <summary>
        /// The message has been sent and seen by the recipient.
        /// </summary>
        Seen
    }
}
