using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Tidbeat.Data;
using Tidbeat.Models;

namespace Tidbeat.Hub {
    public class ChatHub : Microsoft.AspNetCore.SignalR.Hub {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChatHub(ApplicationDbContext context, UserManager<ApplicationUser> userManager) {
            _context = context;
            _userManager = userManager;
        }

        public Task BroadcastMessage(string conversationId, string userId, string message) {
            return Clients.Group(conversationId).SendAsync("broadcastMessage", userId, message);
        }

        public Task JoinChat(string conversationId) {
            return Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
        }

        public Task LeaveChat(string conversationId) {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
        }

        public Task SetSeenMessages(string conversationId, string userId) {
            return Clients.Group(conversationId).SendAsync("setSeenMessages", userId);
        }

        public Task EditMessage(string conversationId, int messageId, string newText) {
            return Clients.Group(conversationId).SendAsync("editMessage", messageId, newText);
        }

        public Task DeleteMessage(string conversationId, int messageId) {
            return Clients.Group(conversationId).SendAsync("deleteMessage", messageId);
        }
    }
}
