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

        public Task BroadcastMessage(int conversationId, string name, string message) {
            return Clients.Group(conversationId.ToString()).SendAsync("broadcastMessage", name, message);
        }

        public Task Echo(string name, string message) =>
            Clients.Client(Context.ConnectionId)
                   .SendAsync("echo", name, $"{message} (echo from server)");

        public Task JoinChat(int conversationId) {
            return Groups.AddToGroupAsync(Context.ConnectionId, conversationId.ToString());
        }

        public Task LeaveChat(int conversationId) {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId.ToString());
        }
    }
}
