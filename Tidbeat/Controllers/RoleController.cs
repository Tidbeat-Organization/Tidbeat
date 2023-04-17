using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Drawing;
using System.Text.Encodings.Web;
using Tidbeat.Data;
using Tidbeat.Enums;
using Tidbeat.Models;



namespace Tidbeat.Controllers
{
    public class RoleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IStringLocalizer<PostsController> _localizer;

        public RoleController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IEmailSender emailSender, IStringLocalizer<PostsController> localizer)
        {
            _context = context;
            //_serviceProvider = serviceProvider;
            _userManager = userManager;
            _emailSender = emailSender;
            _localizer = localizer;
        }

        [Authorize(Roles = "Moderator,Administrator,Admin")]
        [HttpPost]
        public async Task<ActionResult> EditAsync(string userId,string name, string about )
        {
            var dbUser = await _context.Users.FindAsync(userId);
            var user = await _userManager.GetUserIdAsync(dbUser);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                return NotFound();
            }
            if (!string.IsNullOrEmpty(name))
            {
                dbUser.FullName = name;
            }
            if (!string.IsNullOrEmpty(about))
            {
                dbUser.AboutMe = about;
            }
            var result = await _userManager.UpdateAsync(dbUser);
            if (result.Succeeded)
            {
                _context.SaveChanges();
                await _emailSender.SendEmailAsync(dbUser.Email, "TIDBEAT - " + _localizer["account_updated"],
                     _localizer["email_body_edit"]); 
                return Json(_localizer["user_update"]);
            }
            return Json(_localizer["operation_fail"]);
        }

        [Authorize(Roles = "Moderator,Administrator,Admin")]
        [HttpPost]
        public async Task<ActionResult> DeleteAsync(string userId, string reason)
        {
            var dbUser = await _context.Users.FindAsync(userId);
            var user = await _userManager.GetUserIdAsync(dbUser);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                return NotFound();
            }
            dbUser.IsBanned = true;
            dbUser.reason = reason;
            var result = await _userManager.UpdateAsync(dbUser);
            //Falta adicionar o [Deleted] aos posts e comments
            var invalidUser = await _userManager.FindByEmailAsync(Configurations.InvalidUser.Email);

            var postsFromUser = _context.Posts.Where(post => post.User.Email == dbUser.Email);
            foreach (var post in postsFromUser)
            {
                post.User = invalidUser;
            }

            var commentsFromUser = _context.Comment.Where(comment => comment.User.Email == dbUser.Email);
            foreach (var comment in commentsFromUser)
            {
                comment.User = invalidUser;
            }

            var postRatings = _context.PostRatings.Where(postRating => postRating.User.Email == dbUser.Email);
            foreach (var postRating in postRatings)
            {
                postRating.User = invalidUser;
            }

            var commentRatings = _context.CommentRatings.Where(commentRating => commentRating.User.Email == dbUser.Email);
            foreach (var commentRating in commentRatings)
            {
                commentRating.User = invalidUser;
            }

            _context.SaveChanges();
            if (result.Succeeded)
            {
                await _emailSender.SendEmailAsync(dbUser.Email, "TIDBEAT - " + _localizer["account_deleted"],
                    _localizer["email_body_delete"] + reason);
                return Json(_localizer["user_delete"]);
            }
            return Json(_localizer["operation_fail"]);
        }

        [Authorize(Roles = "Moderator,Administrator,Admin")]
        [HttpPost]
        public async Task<ActionResult> BanAsync(string userId, string reason, int time, string date) // date, passes month, day, years, weeks
        {
            var dbUser = await _context.Users.FindAsync(userId);
            var user = await _userManager.GetUserIdAsync(dbUser);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                return NotFound();
            }
            var BanDateEnd = DateTime.Now;
            switch (date) 
            {
                case "month":
                    BanDateEnd = BanDateEnd.AddMonths(time);
                    break;
                case "year":
                    BanDateEnd = BanDateEnd.AddYears(time);
                    break;
                case "day":
                    BanDateEnd = BanDateEnd.AddDays(time);
                    break;
                case "weeks":
                    BanDateEnd = BanDateEnd.AddDays(time * 7);
                    break;
            }
            var BanUser = new BanUser() { EndsAt = BanDateEnd,User = dbUser};
            if (dbUser.Bans == null)
            {
                dbUser.Bans = new List<BanUser>();
            }
            dbUser.Bans.Add(BanUser);
            var result = await _userManager.UpdateAsync(dbUser);
            if (result.Succeeded)
            {
                _context.SaveChanges();
                await _emailSender.SendEmailAsync(dbUser.Email, "TIDBEAT - " + _localizer["account_ban"],
                    _localizer["email_body_ban"] + BanDateEnd);
                return Json(_localizer["user_ban"]);
            }
            return Json(_localizer["operation_fail"]);
        }

        [Authorize(Roles = "Administrator,Admin")]
        [HttpPost]
        public async Task<ActionResult> RevokePermisson(string userId)
        {
            var dbUser = await _context.Users.FindAsync(userId);
            var result = _userManager.RemoveFromRoleAsync(dbUser,dbUser.Role.ToString());
            if (result.IsCompletedSuccessfully)
            {
                var newPermission = _userManager.AddToRoleAsync(dbUser, Enums.RoleType.NormalUser.ToString());
                if (newPermission.IsCompletedSuccessfully) 
                {
                    dbUser.Role = Enums.RoleType.NormalUser;

                    _context.SaveChanges();
                }
            }

            return Json("Error");
        }

        [Authorize(Roles = "Administrator,Admin")]
        [HttpPost]
        public async Task<ActionResult> GivePermisson(string userId, RoleType newRole)
        {
            var dbUser = await _context.Users.FindAsync(userId);
            var result = _userManager.RemoveFromRoleAsync(dbUser, dbUser.Role.ToString());
            if (result.IsCompletedSuccessfully)
            {
                var newPermission = _userManager.AddToRoleAsync(dbUser, newRole.ToString());
                if (newPermission.IsCompletedSuccessfully)
                {
                    dbUser.Role = newRole;

                    _context.SaveChanges();
                }
            }

            return Json("Error");
        }
    }
}
