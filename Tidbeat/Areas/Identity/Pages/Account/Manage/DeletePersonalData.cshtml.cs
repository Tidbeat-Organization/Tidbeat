// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Tidbeat.Controllers;
using Tidbeat.Data;
using Tidbeat.Models;

namespace Tidbeat.Areas.Identity.Pages.Account.Manage
{
    /// <summary>
    ///    The model class for the delete personal data page.
    /// </summary>
    public class DeletePersonalDataModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<DeletePersonalDataModel> _logger;
        private readonly IStringLocalizer<DeletePersonalDataModel> _localizer;
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// The constructor for the delete personal data model.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="signInManager">The sign in manager.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="localizer">The localizer.</param>
        /// <param name="context">The context.</param>
        public DeletePersonalDataModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<DeletePersonalDataModel> logger,
            IStringLocalizer<DeletePersonalDataModel> localizer,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _localizer = localizer;
            _context = context;
        }

        /// <summary>
        /// The input model.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        /// The input model.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            /// The password.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        /// <summary>
        /// The require password.
        /// </summary>
        public bool RequirePassword { get; set; }

        /// <summary>
        /// The on get method.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            RequirePassword = await _userManager.HasPasswordAsync(user);

            if (User.Identity.IsAuthenticated)
            {
                var userr = await _userManager.GetUserAsync(User);
                var request = HttpContext.Request;
                var currentUrl = string.Format("{0}://{1}", request.Scheme, request.Host);
                TempData["Friends"] = await UtilityClass.SideBarAsync(userr.Id, currentUrl);
            }

            return Page();
        }

        /// <summary>
        /// The on post method. Checks if the password is correct and deletes the user.
        /// </summary>
        /// <remarks>When the user is deleted, his posts, comments and ratings are all replaced by a "deleted" user, so that data doesn't get lost.</remarks>
        /// <returns>If its invalid, returns the page itself with errors. If its valid, redirects to the main page.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            RequirePassword = await _userManager.HasPasswordAsync(user);
            if (RequirePassword)
            {
                if (!await _userManager.CheckPasswordAsync(user, Input.Password))
                {
                    ModelState.AddModelError("Input.Password", _localizer["wrong_password"]);
                    return Page();
                }
            }

            var invalidUser = await _userManager.FindByEmailAsync(Configurations.InvalidUser.Email);

            var postsFromUser = _context.Posts.Where(post => post.User.Email == user.Email);
            foreach (var post in postsFromUser) {
                post.User = invalidUser;
            }

            var commentsFromUser = _context.Comment.Where(comment => comment.User.Email == user.Email);
            foreach (var comment in commentsFromUser) {
                comment.User = invalidUser;
            }

            var postRatings = _context.PostRatings.Where(postRating => postRating.User.Email == user.Email);
            foreach (var postRating in postRatings) {
                postRating.User = invalidUser;
            }

            var commentRatings = _context.CommentRatings.Where(commentRating => commentRating.User.Email == user.Email);
            foreach (var commentRating in commentRatings) {
                commentRating.User = invalidUser;
            }

            var userReporters = _context.Report.Where(report => report.UserReporter.Email == user.Email);
            foreach (var report in userReporters) {
                report.UserReporter = invalidUser;
            }

            var userReported = _context.Report.Where(report => report.UserReported.Email == user.Email);
            foreach (var report in userReported) {
                if (report.ReportItemId.Equals(report.UserReported.Id)) {
                    _context.Report.Remove(report);
                    continue;
                }
                report.UserReported = invalidUser;
            }

            var userMessages = _context.Messages.Where(message => message.User.Email == user.Email);
            foreach (var message in userMessages) {
                _context.Messages.Remove(message);
            }

            var userParticipants = _context.Participants.Where(participant => participant.User.Email == user.Email).Include(p => p.Conversation);
            foreach (var participant in userParticipants) {
                var conversation = _context.Conversations.Find(participant.Conversation.Id);
                _context.Participants.Remove(participant);
                _context.Conversations.Remove(conversation);
            }

            var followedUsers = _context.Follow.Where(follow => follow.UserFollowed.Email == user.Email);
            foreach (var follow in followedUsers) {
                _context.Follow.Remove(follow);
            }

            var followingUsers = _context.Follow.Where(follow => follow.UserAsker.Email == user.Email);
            foreach (var follow in followingUsers) {
                _context.Follow.Remove(follow);
            }


            _context.SaveChanges();

            var result = await _userManager.DeleteAsync(user);
            var userId = await _userManager.GetUserIdAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Unexpected error occurred deleting user.");
            }

            await _signInManager.SignOutAsync();

            _logger.LogInformation("User with ID '{UserId}' deleted themselves.", userId);

            return Redirect("~/");
        }

        public static async Task<IdentityResult> DeleteUser(string userIde, ApplicationDbContext _context, UserManager<ApplicationUser> _userManager) {
            
            var invalidUser = await _userManager.FindByEmailAsync(Configurations.InvalidUser.Email);
            var user = await _userManager.FindByIdAsync(userIde);

            var postsFromUser = _context.Posts.Where(post => post.User.Email == user.Email);
            foreach (var post in postsFromUser) {
                post.User = invalidUser;
            }

            var commentsFromUser = _context.Comment.Where(comment => comment.User.Email == user.Email);
            foreach (var comment in commentsFromUser) {
                comment.User = invalidUser;
            }

            var postRatings = _context.PostRatings.Where(postRating => postRating.User.Email == user.Email);
            foreach (var postRating in postRatings) {
                postRating.User = invalidUser;
            }

            var commentRatings = _context.CommentRatings.Where(commentRating => commentRating.User.Email == user.Email);
            foreach (var commentRating in commentRatings) {
                commentRating.User = invalidUser;
            }

            var userReporters = _context.Report.Where(report => report.UserReporter.Email == user.Email);
            foreach (var report in userReporters) {
                report.UserReporter = invalidUser;
            }

            var userReported = _context.Report.Where(report => report.UserReported.Email == user.Email);
            foreach (var report in userReported) {
                if (report.ReportItemId.Equals(report.UserReported.Id)) {
                    _context.Report.Remove(report);
                    continue;
                }
                report.UserReported = invalidUser;
            }

            var userMessages = _context.Messages.Where(message => message.User.Email == user.Email);
            foreach (var message in userMessages) {
                _context.Messages.Remove(message);
            }

            var userParticipants = _context.Participants.Where(participant => participant.User.Email == user.Email).Include(p => p.Conversation);
            foreach (var participant in userParticipants) {
                var conversation = _context.Conversations.Find(participant.Conversation.Id);
                _context.Participants.Remove(participant);
                _context.Conversations.Remove(conversation);
            }

            var followedUsers = _context.Follow.Where(follow => follow.UserFollowed.Email == user.Email);
            foreach (var follow in followedUsers) {
                _context.Follow.Remove(follow);
            }

            var followingUsers = _context.Follow.Where(follow => follow.UserAsker.Email == user.Email);
            foreach (var follow in followingUsers) {
                _context.Follow.Remove(follow);
            }


            _context.SaveChanges();

            return await _userManager.DeleteAsync(user);
            

            
        }
    }
}