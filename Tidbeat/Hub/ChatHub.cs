using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Tidbeat.Data;
using Tidbeat.Models;

namespace Tidbeat.Hub {
    /// <summary>
    /// The SignalR hub for the ChatBeat functionality. This class is only for sending messages to a connection.
    /// </summary>
    /// <remarks>This class does NOT make changes to the database. It only sends information to connections. Meaning, if one user edits a message, that change will
    /// be dynamically rendered in all other users that are connected to the group. However, that edit action will not be added to the database. That action
    /// is a different method in a different class, so it must be called separately.
    /// </remarks>
    public class ChatHub : Microsoft.AspNetCore.SignalR.Hub {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChatHub(ApplicationDbContext context, UserManager<ApplicationUser> userManager) {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Broadcasts a message to all users in a conversation.
        /// </summary>
        /// <param name="conversationId">The id of the conversation.</param>
        /// <param name="userId">The id of the user.</param>
        /// <param name="message">The message.</param>
        /// <returns>The task.</returns>
        public Task BroadcastMessage(string conversationId, string userId, string message) {
            return Clients.Group(conversationId).SendAsync("broadcastMessage", userId, message);
        }

        /// <summary>
        /// When a user joins a conversation, adds them to the group.
        /// </summary>
        /// <param name="conversationId">The id of the conversation.</param>
        /// <returns>The task.</returns>
        public Task JoinChat(string conversationId) {
            return Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
        }

        /// <summary>
        /// When a user leaves a conversation, removes them from the group.
        /// </summary>
        /// <param name="conversationId">The id of the conversation.</param>
        /// <returns>The task.</returns>
        public Task LeaveChat(string conversationId) {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
        }

        /// <summary>
        /// Sets all messages of a user in a conversation to seen.
        /// </summary>
        /// <param name="conversationId">The id of the conversation.</param>
        /// <param name="userId">The id of the user.</param>
        /// <returns>The task.</returns>
        public Task SetSeenMessages(string conversationId, string userId) {
            return Clients.Group(conversationId).SendAsync("setSeenMessages", userId);
        }

        /// <summary>
        /// Sends to all users in a group a edit message request.
        /// </summary>
        /// <param name="conversationId">The id of the conversation.</param>
        /// <param name="messageId">The id of the message.</param>
        /// <param name="newText">The new text of the message.</param>
        /// <returns>The task.</returns>
        public Task EditMessage(string conversationId, int messageId, string newText) {
            return Clients.Group(conversationId).SendAsync("editMessage", messageId, newText);
        }

        /// <summary>
        /// Sends to all users in a group a delete message request.
        /// </summary>
        /// <param name="conversationId">The id of the conversation.</param>
        /// <param name="messageId">The id of the message.</param>
        /// <returns>The task.</returns>
        public Task DeleteMessage(string conversationId, int messageId) {
            return Clients.Group(conversationId).SendAsync("deleteMessage", messageId);
        }

        /// <summary>
        /// Sends the id of the just sent message to the users.
        /// </summary>
        /// <param name="conversationId">The id of the conversation.</param>
        /// <param name="messageId">The id of the message.</param>
        /// <param name="userId">The id of the user.</param>
        /// <returns>The task.</returns>
        public Task AddMessageId(string conversationId, int messageId, string userId) {
            return Clients.Group(conversationId).SendAsync("addMessageId", messageId, userId);
        }

        /// <summary>
        /// Sets the online status for all users in the conversation.
        /// </summary>
        /// <param name="conversationId">The id of the conversation.</param>
        /// <param name="currentUserId">The id of the user which just logged in.</param>
        /// <param name="hasCheckOthersOnlineStatus">A boolean for checking if others have already checked if he was online.</param>
        /// <returns></returns>
        public Task SetOnlineStatus(string conversationId, string currentUserId, bool hasCheckOthersOnlineStatus) {
            return Clients.Group(conversationId).SendAsync("setOnlineStatus", currentUserId, hasCheckOthersOnlineStatus);
        }

        /// <summary>
        /// Sets the status to offline for all users in the conversations.
        /// </summary>
        /// <param name="conversationId">The id of the conversation.</param>
        /// <param name="currentUserId">The id of the user which just logged in.</param>
        /// <returns></returns>
        public Task SetOfflineStatus(string conversationId, string currentUserId) {
            return Clients.Group(conversationId).SendAsync("setOfflineStatus", currentUserId);
        }
    }
}
