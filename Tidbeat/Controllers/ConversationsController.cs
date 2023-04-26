using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Tidbeat.Data;
using Tidbeat.DTOs.ChatBeat;
using Tidbeat.Enums;
using Tidbeat.Hub;
using Tidbeat.Models;
using Tidbeat.Services;

namespace Tidbeat.Controllers
{
    /// <summary>
    /// Controls everything related to the chat system.
    /// </summary>
    public class ConversationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IChatBeatService _chatBeatService;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes needed services for the controller.
        /// </summary>
        /// <param name="context">Context of the website.</param>
        /// <param name="userManager">The user manager.</param>
        /// <param name="chatBeatService">The chatting service.</param>
        public ConversationsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IChatBeatService chatBeatService, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _chatBeatService = chatBeatService;
            _configuration = configuration;
        }

        /// <summary>
        /// The index view with all conversations a user has.
        /// </summary>
        /// <remarks>GET: Conversations/</remarks>
        /// <returns>The index view with all conversations a user has. If the user is not logged in, the user is redirected to the login page.</returns>
        // GET: Conversations
        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated) {
                return Redirect("/Identity/Account/Login");
            }
            var currentUser = await _userManager.GetUserAsync(User);
            var allCurrentUserConversations = await _context.Participants.Where(p => p.User.Id == currentUser.Id).Select(p => p.Conversation).ToListAsync();
            var allParticipantsInConversations = await _context.Participants.Include(p => p.User).Where(p => allCurrentUserConversations.Contains(p.Conversation) && p.User != currentUser).ToListAsync();

            var conversationsParticipantsPairs = new List<Tuple<Conversation, List<string>, int>>();
            foreach (var conversation in allCurrentUserConversations) {
                var participants = allParticipantsInConversations.Where(p => p.Conversation == conversation).Select(p => p.User.FullName).ToList();
                var unreadMessagesCount = await _context.Messages
                        .Include(m => m.Conversation)
                        .Include(m => m.User)
                        .Where(m => m.Conversation.Id == conversation.Id && m.Status == Convert.ToInt32(MessageStatus.Sent) && currentUser.Id != m.User.Id)
                        .CountAsync();
                conversationsParticipantsPairs.Add(new Tuple<Conversation, List<string>, int>(conversation, participants, unreadMessagesCount));
            }
            ViewBag.ConversationsParticipantsPairs = conversationsParticipantsPairs;

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                var request = HttpContext.Request;
                var currentUrl = string.Format("{0}://{1}", request.Scheme, request.Host);
                TempData["Friends"] = await UtilityClass.SideBarAsync(user.Id, currentUrl);
            }

            return _context.Conversations != null ? 
                          View(await _context.Conversations.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Conversations'  is null.");
        }

        /// <summary>
        /// The details view of a conversation. Shows all messages in the conversation.
        /// </summary>
        /// <param name="id">The id of the conversation.</param>
        /// <remarks>GET: Conversations/Details/{id}  
        /// In this view, its being used SignalR for establishing the connection between all other users.</remarks>
        /// <returns>The Details view of the conversation. If the conversation doesn't exist, returns a 404 page.</returns>
        // GET: Conversations/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            const int messageAmountPerCall = 20;
            if (id == null || _context.Conversations == null)
            {
                return NotFound();
            }

            var conversation = await _context.Conversations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (conversation == null)
            {
                return NotFound();
            }
            var currentUser = await _userManager.GetUserAsync(User);
            ViewBag.CurrentUser = currentUser;
            ViewBag.OtherUser = _context.Participants.Where(p => p.Conversation.Id == id && p.User.Id != currentUser.Id).Select(p => p.User).FirstOrDefault();

            ViewBag.MessageAmountPerCall = messageAmountPerCall;

            var topMessages = await _chatBeatService.GetRecentMessages(id, messageAmountPerCall, 0);
            topMessages.Reverse();
            ViewBag.Messages = topMessages;

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                var request = HttpContext.Request;
                var currentUrl = string.Format("{0}://{1}", request.Scheme, request.Host);
                TempData["Friends"] = await UtilityClass.SideBarAsync(user.Id, currentUrl);
            }

            return View(conversation);
        }

        /// <summary>
        /// Changes the message status to seen.
        /// </summary>
        /// <param name="setMessageToSeenDto">DTO which has the user's and conversation's id.</param>
        /// <remarks>PUT: Conversations/SetUsersMessagesToSeen</remarks>
        /// <returns>An Ok if successful.</returns>
        [HttpPut]
        public async Task<IActionResult> SetUsersMessagesToSeen([FromBody] SetMessageToSeenDto setMessageToSeenDto) {
            await _chatBeatService.SetUsersMessagesToSeen(setMessageToSeenDto.ConversationId, setMessageToSeenDto.UserId);
            return Ok();
        }

        /// <summary>
        /// Gets the most recent messages based on an offset.
        /// </summary>
        /// <param name="getRecentMessageDto">DTO which has the user's id, the amount of messages to get and the amount of messages to skip.</param>
        /// <remarks>POST: Conversations/GetRecentMessages</remarks>
        /// <returns>Um json com todas as mensagens obtidas.</returns>
        public async Task<IActionResult> GetRecentMessages([FromBody] GetRecentMessageDto getRecentMessageDto) {
            var messages = await _chatBeatService.GetRecentMessages(getRecentMessageDto.ConversationId, getRecentMessageDto.MessageAmount, getRecentMessageDto.SkipAmount);
            return Json(messages);
        }

        /// <summary>
        /// Saves a message in the database.
        /// </summary>
        /// <param name="messageDto">DTO which has the conversation's and user's id and the text of the message.</param>
        /// <remarks> POST: Conversations/SaveMessage</remarks>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SaveMessage([FromBody] MessageDto messageDto) {
            var messageId = await _chatBeatService.AddMessageToDatabase(messageDto.ConversationId, messageDto.UserId, messageDto.Text);
            var customResponse = new {
                MessageId = messageId
            };
            return Ok(customResponse);
        }

        /// <summary>
        /// Edits a message in the database.
        /// </summary>
        /// <param name="editMessageDto">DTO which has the user's and message's id and the new text of the message.</param>
        /// <remarks>PUT: Conversations/EditMessage</remarks>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> EditMessage([FromBody] EditMessageDto editMessageDto) {
            await _chatBeatService.EditMessageInDatabase(editMessageDto.UserId, editMessageDto.MessageId, editMessageDto.Text);
            return Ok();
        }

        /// <summary>
        /// Deletes a message from the database.
        /// </summary>
        /// <param name="deleteMessageDto">DTO which has the user's and message's id.</param>
        /// <remarks>DELETE: Conversations/DeleteMessage</remarks>
        /// <returns>Ok if successful.</returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteMessage([FromBody] DeleteMessageDto deleteMessageDto) {
            await _chatBeatService.RemoveMessageFromDatabase(deleteMessageDto.MessageId, deleteMessageDto.UserId);
            return Ok();
        }

        /// <summary>
        /// Gets the conversation between two users. If it doesn't exist, creates it.
        /// </summary>
        /// <param name="currentUserId">The id of the user.</param>
        /// <param name="otherUserId">The id of the other user.</param>
        /// <returns>A redirect to the details of a conversation.</returns>
        public async Task<IActionResult> StartTwoPersonConversation(string currentUserId, string otherUserId) {
            var conversation = await _chatBeatService.GetTwoPersonConversation(currentUserId, otherUserId);
            return RedirectToAction("Details", new { id = conversation.Id });
        }

        // GET: Conversations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Conversations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,StartDate")] Conversation conversation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(conversation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(conversation);
        }

        // GET: Conversations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Conversations == null)
            {
                return NotFound();
            }

            var conversation = await _context.Conversations.FindAsync(id);
            if (conversation == null)
            {
                return NotFound();
            }
            return View(conversation);
        }

        // POST: Conversations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Title,StartDate")] Conversation conversation)
        {
            if (id != conversation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(conversation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConversationExists(conversation.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(conversation);
        }

        /// <summary>
        /// The user exits a conversation. If there are only 2 participants, the conversation is deleted. Otherwise, the user is removed from the conversation.
        /// </summary>
        /// <param name="conversationId">The id of the conversation to exit from.</param>
        /// <returns>A redirect to the list of the conversations.</returns>
        public async Task<IActionResult> ExitConversation(string conversationId) {
            var currentUser = await _userManager.GetUserAsync(User);
            var conversation = await _context.Conversations.FindAsync(conversationId);
            var participants = await _context.Participants.Where(p => p.Conversation == conversation).ToListAsync();
            if (participants.Count == 2) {
                _context.Conversations.Remove(conversation);
            }
            else {
                var participant = await _context.Participants.Where(p => p.Conversation == conversation && p.User == currentUser).FirstOrDefaultAsync();
                _context.Participants.Remove(participant);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Conversations");
        }

        // GET: Conversations/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null || _context.Conversations == null)
            {
                return NotFound();
            }

            var conversation = await _context.Conversations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (conversation == null)
            {
                return NotFound();
            }

            return View(conversation);
        }

        // POST: Conversations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Conversations == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Conversations'  is null.");
            }
            var conversation = await _context.Conversations.FindAsync(id);
            if (conversation != null)
            {
                _context.Conversations.Remove(conversation);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<int> GetMessagesNotReadAmount() {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) {
                return 0;
            }

            int totalAmount = 0;
            foreach(Conversation conversation in _context.Conversations) {
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
                var currentUserParticipant = 0;
                using (var context = new ApplicationDbContext(optionsBuilder.Options)) {
                    currentUserParticipant = await context.Participants
                        .Include(m => m.User)
                        .Include(p => p.Conversation)
                        .Where(p => p.Conversation.Id == conversation.Id && p.User.Id == currentUser.Id)
                        .CountAsync();

                    if (currentUserParticipant != 0) {
                        var totalUnreadMessages = await context.Messages
                            .Include(m => m.Conversation)
                            .Include(m => m.User)
                            .Where(m => m.Conversation.Id == conversation.Id && m.Status == Convert.ToInt32(MessageStatus.Sent) && currentUser.Id != m.User.Id)
                            .CountAsync();
                        totalAmount += totalUnreadMessages;
                    }
                }
            }

            return totalAmount;
        }

        private bool ConversationExists(string id)
        {
          return (_context.Conversations?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
