﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Text.Encodings.Web;
using Tidbeat.Data;
using Tidbeat.Models;


namespace Tidbeat.Controllers
{
    public class RoleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public RoleController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _context = context;
            //_serviceProvider = serviceProvider;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [Authorize(Roles = "Moderator,Administrator")]
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
                await _emailSender.SendEmailAsync(dbUser.Email, "TIDBEAT - Account Updated",
                    "Viemos informar que a sua conta foi modificada, por não cumprir as diretivas");
                return Json("UserUpdated");
            }
            return Json("Operation Failed");
        }

        [Authorize(Roles = "Moderator,Administrator")]
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
            if (result.Succeeded)
            {
                await _emailSender.SendEmailAsync(dbUser.Email, "TIDBEAT - Account Deleted",
                    "Viemos informar que a sua conta encontra-se banida, pelo seguinte motivo " + reason + "\nPara resgatar a sua conta contactar: ");
                return Json("UserDeleted");
            }
                return Json("Operation Failed");
        }

        [Authorize(Roles = "Moderator,Administrator")]
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
                await _emailSender.SendEmailAsync(dbUser.Email, "TIDBEAT - Account Temporary Banned",
                    "Viemos informar que a sua conta foi temporiariamente banida, por não cumprir as diretivas até:" + BanDateEnd);
                return Json("UserUpdatedTempBan");
            }
            return Json("Operation Failed");
        }
    }
}