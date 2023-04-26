using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Drawing;
using System.Text.Encodings.Web;
using Tidbeat.Areas.Identity.Pages.Account.Manage;
using Tidbeat.Data;
using Tidbeat.DTOs.Role;
using Tidbeat.Enums;
using Tidbeat.Models;



namespace Tidbeat.Controllers
{
    /// <summary>
    /// A controller used for the behaviour of the administrator's definition page on the user page.
    /// </summary>
    public class RoleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IStringLocalizer<RoleController> _localizer;
        private readonly SignInManager<ApplicationUser> _signInManager;

        /// <summary>
        /// The controller for the RoleController.
        /// </summary>
        /// <param name="context">The context of the application.</param>
        /// <param name="userManager">The user manager object.</param>
        /// <param name="emailSender">The email sender object.</param>
        /// <param name="localizer">The language localizer object.</param>
        /// <param name="signInManager">The sign in manager object.</param>
        public RoleController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IEmailSender emailSender, IStringLocalizer<RoleController> localizer, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            //_serviceProvider = serviceProvider;
            _userManager = userManager;
            _emailSender = emailSender;
            _localizer = localizer;
            _signInManager = signInManager;
        }

        /// <summary>
        /// The method which edits the main user definitions like the full name and the about me, the banning values, if it should delete the photo and the new role.
        /// </summary>
        /// <param name="editAsyncDto">A data transfer object for making it easier to convert from the body to parameters.</param>
        /// <returns></returns>
        [Authorize(Roles = "Moderator,Admin")]
        [HttpPost]
        public async Task<ActionResult> EditAsync([FromBody] EditAsyncDto editAsyncDto )
        {
            var userId = editAsyncDto.UserId;
            var name = editAsyncDto.Name;
            var about = editAsyncDto.About;

            var dbUser = await _userManager.FindByIdAsync(userId);
            if (dbUser == null)
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

            if (editAsyncDto.ShouldDeletePhoto != null && (bool) editAsyncDto.ShouldDeletePhoto) {
                dbUser.ImagePath = "";
            }

            bool hasBanned = false;
            BanTime banTime;
            if (editAsyncDto.BanTime == BanTime.Days.ToString()) {
                banTime = BanTime.Days;
            }
            else if (editAsyncDto.BanTime == BanTime.Weeks.ToString()) {
                banTime = BanTime.Weeks;
            }
            else if (editAsyncDto.BanTime == BanTime.Months.ToString()) {
                banTime = BanTime.Months;
            }
            else if (editAsyncDto.BanTime == BanTime.Years.ToString()) {
                banTime = BanTime.Years;
            }
            else if (editAsyncDto.BanTime == BanTime.Minutes.ToString()) {
                banTime = BanTime.Minutes;
            }
            else {
                banTime = BanTime.Hours;
            }
            if (editAsyncDto.BanNumber != null) {
                _ = await BanAsync(userId, editAsyncDto.BanReason, (double)editAsyncDto.BanNumber, banTime);
                hasBanned = true;
            }

            RoleType roleType;
            bool hasRole = false;
            if (editAsyncDto.RoleType == RoleType.Admin.ToString()) {
                roleType = RoleType.Admin;
                hasRole = true;
            }
            else if (editAsyncDto.RoleType == RoleType.Moderator.ToString()) {
                roleType = RoleType.Moderator;
                hasRole = true;
            }
            else {
                roleType = RoleType.NormalUser;
            }

            _ = await GivePermisson(userId, roleType);

            var result = await _userManager.UpdateAsync(dbUser);
            if (result.Succeeded && _emailSender != null) {
                _context.SaveChanges();
                if (!hasBanned && !hasRole) {
                    await _emailSender.SendEmailAsync(dbUser.Email, "TIDBEAT - " + _localizer["account_updated"],
                         _localizer["email_body_edit"]);
                }
                //return Json(_localizer["user_update"]);
            }

            return Json(_localizer["operation_fail"]);
        }

        /// <summary>
        /// The action for deleting the user in the administrator's definitions page.
        /// </summary>
        /// <param name="deleteAsyncDto">The data transfer object for deleting a user.</param>
        /// <returns>A json about whether the user deletion was successful.</returns>
        [Authorize(Roles = "Moderator,Admin")]
        [HttpPost]
        public async Task<ActionResult> DeleteAsync([FromBody] DeleteAsyncDto deleteAsyncDto)
        {
            var userId = deleteAsyncDto.UserId;
            var reason = deleteAsyncDto.Reason;
            var dbUser = await _context.Users.FindAsync(userId);
            var user = await _userManager.GetUserIdAsync(dbUser);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            var result = await DeletePersonalDataModel.DeleteUser(userId, _context, _userManager);
            
            if (result.Succeeded && _emailSender != null)
            {
                await _emailSender.SendEmailAsync(dbUser.Email, "TIDBEAT - " + _localizer["account_deleted"],
                    _localizer["email_body_delete"] + reason);
                return Json(_localizer["user_delete"]);
            }
            return Json(_localizer["operation_fail"]);
        }

        /// <summary>
        /// An action for banning users.
        /// </summary>
        /// <param name="userId">The id of the user about to be banned.</param>
        /// <param name="reason">The reason for banning.</param>
        /// <param name="time">The length of time which a user is supposed to be banned for.</param>
        /// <param name="date">The type of time: minutes, hours, days, weeks, months, years</param>
        /// <returns>A JSON of whether the action was successful or not.</returns>
        [Authorize(Roles = "Moderator,Admin")]
        [HttpPost]
        public async Task<ActionResult> BanAsync(string userId, string reason, double time, BanTime date) // date, passes month, day, years, weeks
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
                case BanTime.Minutes:
                    BanDateEnd = BanDateEnd.AddMinutes((int)Math.Floor(time));
                    break;
                case BanTime.Hours:
                    BanDateEnd = BanDateEnd.AddHours((int)Math.Floor(time));
                    break;
                case BanTime.Months:
                    BanDateEnd = BanDateEnd.AddMonths((int) Math.Floor(time));
                    break;
                case BanTime.Years:
                    BanDateEnd = BanDateEnd.AddYears((int) Math.Floor(time));
                    break;
                case BanTime.Days:
                    BanDateEnd = BanDateEnd.AddDays(time);
                    break;
                case BanTime.Weeks:
                    BanDateEnd = BanDateEnd.AddDays(time * 7);
                    break;
            }
            var BanUser = new BanUser() {
                EndsAt = BanDateEnd,
                User = dbUser,
                Reason = reason,
            };
            if (dbUser.Bans == null)
            {
                dbUser.Bans = new List<BanUser>();
            }
            dbUser.Bans.Add(BanUser);
            dbUser.IsBanned = true;
            var result = await _userManager.UpdateAsync(dbUser);
            if (result.Succeeded)
            {
                _context.SaveChanges();
                await _emailSender.SendEmailAsync(dbUser.Email, "TIDBEAT - " + _localizer["account_ban"],
                    _localizer["email_body_ban"] + BanDateEnd.ToString("dd/MM/yyyy HH:mm:ss"));
                return Json(_localizer["user_ban"]);
            }
            return Json(_localizer["operation_fail"]);
        }

        /// <summary>
        /// The action which concedes a new role to the user.
        /// </summary>
        /// <param name="userId">The id of the user about to get a new role.</param>
        /// <param name="newRole">The type of the new role about to be conceded.</param>
        /// <returns>A JSON about the type of error returned.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> GivePermisson(string userId, RoleType newRole)
        {
            var dbUser = await _context.Users.FindAsync(userId);
            if (dbUser.Role == newRole) {
                return Json("Both roles are the same.");
            }
            var result = await _userManager.RemoveFromRoleAsync(dbUser, dbUser.Role.ToString());
            if (result.Succeeded)
            {
                var newPermission = await _userManager.AddToRoleAsync(dbUser, newRole.ToString());
                if (newPermission.Succeeded)
                {
                    var actualUser = await _userManager.FindByIdAsync(userId);
                    actualUser.Role = newRole;
                    _context.SaveChanges();
                }
            }

            return Json("Error");
        }
    }
}
