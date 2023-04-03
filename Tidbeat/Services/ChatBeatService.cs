﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tidbeat.Data;
using Tidbeat.DTOs;
using Tidbeat.Enums;
using Tidbeat.Models;

namespace Tidbeat.Services {
    public class ChatBeatService : IChatBeatService {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public ChatBeatService(ApplicationDbContext context, UserManager<ApplicationUser> userManager) {
            _userManager = userManager;
            _context = context;
        }

        public async Task AddMessageToDatabase(string conversationId, string userId, string text) {
            var message = new Message {
                Text = text,
                Status = (int)MessageStatus.Sent,
                Created = DateTime.Now,
                User = await _userManager.FindByIdAsync(userId),
                Conversation = await _context.Conversations.FirstOrDefaultAsync(c => c.Id == conversationId)
            };
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
        }

        public Task AddPersonToConversation(string conversationId, string userId) {
            throw new NotImplementedException();
        }

        public Task<Conversation> GetConversation(string conversationId) {
            throw new NotImplementedException();
        }

        public Task<List<Message>> GetRecentMessages(string conversationId, int messageAmount, int skipAmount) {
            var messages = _context.Messages
                .Where(m => m.Conversation.Id == conversationId)
                .Include(c => c.User)
                .OrderByDescending(m => m.Created)
                .Skip(skipAmount)
                .Take(messageAmount)
                .ToListAsync();
            return messages;
        }

        public async Task<Conversation> GetTwoPersonConversation(string currentUserId, string otherUserId) {
            var allCurrentUserConversations = _context.Participants.Where(p => p.User.Id == currentUserId).Select(p => p.Conversation).ToList();
            var allOtherUserConversations = _context.Participants.Where(p => p.User.Id == otherUserId).Select(p => p.Conversation).ToList();
            var commonConversations = allCurrentUserConversations.Intersect(allOtherUserConversations).ToList();

            var foundConversation = await _context.Participants
                .Where(p => commonConversations.Contains(p.Conversation) && !p.Conversation.IsGroupConversation && (p.User.Id == currentUserId || p.User.Id == otherUserId))
                .Select(p => p.Conversation)
                .Distinct()
                .ToListAsync();

            if (foundConversation.Count > 1) {
                throw new Exception("There shouldn't be more than one exclusive conversation with one user.");
            }
            else if (foundConversation.Count == 1) {
                return foundConversation[0];
            }
            else {
                var conversation = new Conversation {
                    Id = Guid.NewGuid().ToString(),
                    StartDate = DateTime.Now,
                    IsGroupConversation = false
                };
                _context.Conversations.Add(conversation);

                var currentParticipant = new Participant {
                    Conversation = conversation,
                    User = await _userManager.FindByIdAsync(currentUserId)
                };
                _context.Participants.Add(currentParticipant);

                var otherParticipant = new Participant {
                    Conversation = conversation,
                    User = await _userManager.FindByIdAsync(otherUserId)
                };
                _context.Participants.Add(otherParticipant);

                await _context.SaveChangesAsync();
                return conversation;
            }
        }

        public async Task EditMessageInDatabase(string userId, int messageId, string text) {
            var message = await _context.Messages.Include(m => m.User).FirstOrDefaultAsync(m => m.Id == messageId);
            if (message.User.Id != userId)
                throw new Exception("You can't edit a message that isn't yours.");
            message.Text = text;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveMessageFromDatabase(int messageId, string userId) {
            var message = await _context.Messages.Include(m => m.User).FirstOrDefaultAsync(m => m.Id == messageId);
            if (message.User.Id != userId)
                throw new Exception("You can't remove a message that isn't yours.");
            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
        }

        public Task RemovePersonFromConversation(string conversationId, string userId) {
            throw new NotImplementedException();
        }

        public async Task SetUsersMessagesToSeen(string conversationId, string userId) {
            (await _context.Messages
                .Where(m => m.Conversation.Id == conversationId && m.User.Id == userId && m.Status == (int)MessageStatus.Sent)
                .ToListAsync())
                .ForEach(m => m.Status = (int)MessageStatus.Seen);
            await _context.SaveChangesAsync();
        }
    }
}