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
    public class ConversationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IChatBeatService _chatBeatService;

        public ConversationsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IChatBeatService chatBeatService)
        {
            _context = context;
            _userManager = userManager;
            _chatBeatService = chatBeatService;
        }

        // GET: Conversations
        public async Task<IActionResult> Index()
        {
              return _context.Conversations != null ? 
                          View(await _context.Conversations.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Conversations'  is null.");
        }

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

            return View(conversation);
        }

        public async Task<IActionResult> GetRecentMessages([FromBody] GetRecentMessageDto getRecentMessageDto) {
            var messages = await _chatBeatService.GetRecentMessages(getRecentMessageDto.ConversationId, getRecentMessageDto.MessageAmount, getRecentMessageDto.SkipAmount);
            return Json(messages);
        }

        [HttpPost]
        public async Task SaveMessage([FromBody] MessageDto messageDto) {
            await _chatBeatService.AddMessageToDatabase(messageDto.ConversationId, messageDto.UserId, messageDto.Text);
        }

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

        private bool ConversationExists(string id)
        {
          return (_context.Conversations?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
