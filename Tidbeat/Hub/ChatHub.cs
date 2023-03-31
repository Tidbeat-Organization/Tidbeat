﻿using Microsoft.AspNetCore.Identity;
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

        public Task BroadcastMessage(string conversationId, string name, string message) {
            return Clients.Group(conversationId).SendAsync("broadcastMessage", name, message);
        }

        public Task Echo(string name, string message) =>
            Clients.Client(Context.ConnectionId)
                   .SendAsync("echo", name, $"{message} (echo from server)");

        public Task JoinChat(string conversationId) {
            return Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
        }

        public Task LeaveChat(string conversationId) {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
        }
    }
}