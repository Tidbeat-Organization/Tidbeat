using Tidbeat.Models;

namespace Tidbeat.Services {
    public interface IConversationsService {
        Task<Conversation> StartTwoPersonConversation(string currentUserId, string otherUserId);
        Task<Conversation> GetConversation(string conversationId);
    }
}
