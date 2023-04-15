using Tidbeat.Models;

namespace Tidbeat.Services {
    /// <summary>
    /// The chat beat service. This service is used to handle all the chat related logic.
    /// </summary>
    public interface IChatBeatService {
        /// <summary>
        /// Gets a two person conversation. If it doesn't exist, it creates it.
        /// </summary>
        /// <param name="currentUserId">The id of the user that selected the two person conversation.</param>
        /// <param name="otherUserId">The id of the other user.</param>
        /// <returns>The conversation.</returns>
        Task<Conversation> GetTwoPersonConversation(string currentUserId, string otherUserId);

        /// <summary>
        /// Adds a new participant to a conversation.
        /// UNIMPLEMENTED: PREPARED FOR GROUP CONVERSATIONS
        /// </summary>
        /// <param name="conversationId"></param>
        /// <returns></returns>
        Task<Conversation> GetConversation(string conversationId);

        /// <summary>
        /// Adds a new participant to a conversation.
        /// UNIMPLEMENTED: PREPARED FOR GROUP CONVERSATIONS
        /// </summary>
        /// <param name="conversationId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task AddPersonToConversation(string conversationId, string userId);

        /// <summary>
        /// Removes a participant from a conversation.
        /// UNIMPLEMENTED: PREPARED FOR GROUP CONVERSATIONS
        /// </summary>
        /// <param name="conversationId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task RemovePersonFromConversation(string conversationId, string userId);

        /// <summary>
        /// Adds a message to the database.
        /// </summary>
        /// <param name="conversationId">The id of the conversation.</param>
        /// <param name="userId">The id of the user.</param>
        /// <param name="message">The text of the message.</param>
        /// <returns></returns>
        Task<int> AddMessageToDatabase(string conversationId, string userId, string message);

        /// <summary>
        /// Edits a message in the database.
        /// </summary>
        /// <param name="userId">The id of the user which is the owner of the message.</param>
        /// <param name="messageId">The id of the message.</param>
        /// <param name="text">The new text of the message.</param>
        /// <returns></returns>
        Task EditMessageInDatabase(string userId, int messageId, string text);

        /// <summary>
        /// Removes a message from the database.
        /// </summary>
        /// <param name="messageId">The id of the message.</param>
        /// <param name="userId">The id of the user which is the owner of the message.</param>
        /// <returns></returns>
        Task RemoveMessageFromDatabase(int messageId, string userId);

        /// <summary>
        /// Gets all the recent messages in a conversation based on a skip amount and a message amount.
        /// </summary>
        /// <param name="conversationId">The id of the conversation.</param>
        /// <param name="messageAmount">The amount of messages to get.</param>
        /// <param name="skipAmount">The amount of messages to skip.</param>
        /// <returns>A list of the most recent found messages.</returns>
        Task<List<Message>> GetRecentMessages(string conversationId, int messageAmount, int skipAmount);
        /// <summary>
        /// Sets all the messages of a user in a conversation to seen.
        /// </summary>
        /// <param name="conversationId">The id of the conversation.</param>
        /// <param name="userId">The id of the user which will have its message being seen.</param>
        /// <returns></returns>
        Task SetUsersMessagesToSeen(string conversationId, string userId);
    }
}
