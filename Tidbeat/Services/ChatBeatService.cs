using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tidbeat.Data;
using Tidbeat.DTOs;
using Tidbeat.Enums;
using Tidbeat.Models;

namespace Tidbeat.Services {
    /// <summary>
    /// The service that handles all the logic for the chat.
    /// </summary>
    public class ChatBeatService : IChatBeatService {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes the needed services for the service.
        /// </summary>
        /// <param name="context">The context of the application.</param>
        /// <param name="userManager">The user manager.</param>
        public ChatBeatService(ApplicationDbContext context, UserManager<ApplicationUser> userManager) {
            _userManager = userManager;
            _context = context;
        }

        /// <summary>
        /// Adds a message to the database.
        /// </summary>
        /// <param name="conversationId">The id of the conversation.</param>
        /// <param name="userId">The id of the user.</param>
        /// <param name="text">The text of the message.</param>
        /// <returns></returns>
        public async Task<int> AddMessageToDatabase(string conversationId, string userId, string text) {
            var message = new Message {
                Text = text,
                Status = (int)MessageStatus.Sent,
                Created = DateTime.Now,
                User = await _userManager.FindByIdAsync(userId),
                Conversation = await _context.Conversations.FirstOrDefaultAsync(c => c.Id == conversationId)
            };
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return message.Id;
        }

        /// <summary>
        /// Adds a new participant to a conversation.
        /// UNIMPLEMENTED: PREPARED FOR GROUP CONVERSATIONS
        /// </summary>
        /// <param name="conversationId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task AddPersonToConversation(string conversationId, string userId) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds a new participant to a conversation.
        /// UNIMPLEMENTED: PREPARED FOR GROUP CONVERSATIONS
        /// </summary>
        /// <param name="conversationId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<Conversation> GetConversation(string conversationId) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets all the recent messages in a conversation based on a skip amount and a message amount.
        /// </summary>
        /// <param name="conversationId">The id of the conversation.</param>
        /// <param name="messageAmount">The amount of messages to get.</param>
        /// <param name="skipAmount">The amount of messages to skip.</param>
        /// <returns>A list of the most recent found messages.</returns>
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

        /// <summary>
        /// Gets a two person conversation. If it doesn't exist, it creates it.
        /// </summary>
        /// <param name="currentUserId">The id of the user that selected the two person conversation.</param>
        /// <param name="otherUserId">The id of the other user.</param>
        /// <returns>The conversation.</returns>
        /// <exception cref="Exception"></exception>
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

        /// <summary>
        /// Edits a message in the database.
        /// </summary>
        /// <param name="userId">The id of the user which is the owner of the message.</param>
        /// <param name="messageId">The id of the message.</param>
        /// <param name="text">The new text of the message.</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task EditMessageInDatabase(string userId, int messageId, string text) {
            var message = await _context.Messages.Include(m => m.User).FirstOrDefaultAsync(m => m.Id == messageId);
            if (message.User.Id != userId)
                throw new Exception("You can't edit a message that isn't yours.");
            message.Text = text;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Removes a message from the database.
        /// </summary>
        /// <param name="messageId">The id of the message.</param>
        /// <param name="userId">The id of the user which is the owner of the message.</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task RemoveMessageFromDatabase(int messageId, string userId) {
            var message = await _context.Messages.Include(m => m.User).FirstOrDefaultAsync(m => m.Id == messageId);
            if (message == null || message.User == null || userId == null) {
                return;
            }
            if (message.User.Id != userId)
                throw new Exception("You can't remove a message that isn't yours.");
            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Removes a participant from a conversation.
        /// UNIMPLEMENTED: PREPARED FOR GROUP CONVERSATIONS
        /// </summary>
        /// <param name="conversationId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task RemovePersonFromConversation(string conversationId, string userId) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets all the messages of a user in a conversation to seen.
        /// </summary>
        /// <param name="conversationId">The id of the conversation.</param>
        /// <param name="userId">The id of the user which will have its message being seen.</param>
        /// <returns></returns>
        public async Task SetUsersMessagesToSeen(string conversationId, string userId) {
            (await _context.Messages
                .Where(m => m.Conversation.Id == conversationId && m.User.Id == userId && m.Status == (int)MessageStatus.Sent)
                .ToListAsync())
                .ForEach(m => m.Status = (int)MessageStatus.Seen);
            await _context.SaveChangesAsync();
        }
    }
}
