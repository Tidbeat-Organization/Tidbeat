using Tidbeat.Models;

namespace Tidbeat.Services {
    public interface IChatBeatService {
        Task<Conversation> GetTwoPersonConversation(string currentUserId, string otherUserId);
        Task<Conversation> GetConversation(string conversationId);
        Task AddPersonToConversation(string conversationId, string userId);
        Task RemovePersonFromConversation(string conversationId, string userId);
        Task AddMessageToDatabase(string conversationId, string userId, string message);
        Task EditMessageInDatabase(string conversationId, string userId, int messageId, string text);
        Task RemoveMessageFromDatabase(int messageId, string userId);
        Task<List<Message>> GetRecentMessages(string conversationId, int messageAmount, int skipAmount);
        Task SetUsersMessagesToSeen(string conversationId, string userId);
    }
}
