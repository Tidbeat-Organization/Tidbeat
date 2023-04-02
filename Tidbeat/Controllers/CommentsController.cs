using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tidbeat.Data;
using Tidbeat.Models;

namespace Tidbeat.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IServiceProvider _serviceProvider;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentsController(ApplicationDbContext context, IServiceProvider serviceProvider)
        {
            _context = context;
            _serviceProvider = serviceProvider;
            _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        }


        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CommentId,Content")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (User?.Identity.IsAuthenticated == true)
                {
                    if (Int32.TryParse(Request.Form["PostId"], out int checker))
                    {
                        var post = await _context.Posts.FindAsync(checker);
                        if (post != null)
                        {
                            comment.post = post;
                            comment.User = user;
                            comment.CreationDate = DateTime.Now;
                            _context.Add(comment);
                            await _context.SaveChangesAsync();
                            return Redirect("/Posts/Details/" + post.PostId);
                        }
                    }
                }
            }
            return Redirect("/Posts/Details/" + Request.Form["PostId"]);
        }

        // GET: Posts/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Comment == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment.Include(c => c.post).FirstOrDefaultAsync(c => c.CommentId == id);
            if (comment == null)
            {
                return NotFound();
            }
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("CommentId,Content")] Comment comment)
        {
            if (id != comment.CommentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (User?.Identity.IsAuthenticated == true)
                    {
                        var user = await _userManager.GetUserAsync(User);
                        var commentStored = await _context.Comment.Include(c => c.post).FirstOrDefaultAsync(c => c.CommentId == comment.CommentId);
                        if (commentStored != null) {
                            if (user.Id == commentStored.User.Id || _userManager.IsInRoleAsync(user, "Administrator").Result || _userManager.IsInRoleAsync(user, "Moderator").Result) //Add for Roles
                            {
                                commentStored.Content = comment.Content;
                                commentStored.IsEdited = true;
                                commentStored.EditDate = DateTime.Now;
                                _context.Update(commentStored);
                                await _context.SaveChangesAsync();
                            } 
                        }
                        return Redirect("../../Posts/Details/" + commentStored.post.PostId);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.CommentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Posts");
            }
            return RedirectToAction("Index", "Posts");
        }


        // POST: Comments/Delete/5
        // Check user
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Comment == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Comment'  is null.");
            }

            var comment = await _context.Comment.Include(c => c.post).Include(v => v.User).FirstOrDefaultAsync(c => c.CommentId == id);
            var user = await _userManager.GetUserAsync(User);
            if (user.Id == comment.User.Id || _userManager.IsInRoleAsync(user, "Moderator").Result || _userManager.IsInRoleAsync(user, "Administrator").Result) //Add for Roles
            {
                var ratings = await _context.CommentRatings.Where(cr => cr.Comment.CommentId == id).ToListAsync();

                foreach (var rating in ratings)
                {
                    _context.CommentRatings.Remove(rating);
                }
                if (comment != null)
                {
                    _context.Comment.Remove(comment);
                }

                await _context.SaveChangesAsync();
                return Redirect("/Posts/Details/" + comment.post.PostId.ToString());
            }
            return NotFound();
        }

        private bool CommentExists(int id)
        {
          return _context.Comment.Any(e => e.CommentId == id);
        }
    }
}
