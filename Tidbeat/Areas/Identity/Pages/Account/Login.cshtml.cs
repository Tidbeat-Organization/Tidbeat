// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;
using Tidbeat.Models;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Tidbeat.Data;
using Microsoft.EntityFrameworkCore;

namespace Tidbeat.Areas.Identity.Pages.Account
{
    /// <summary>
    /// The model class for the login page.
    /// </summary>
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private static string Pattern = RegisterModel.Pattern;
        private readonly IStringLocalizer<LoginModel> _localizer;
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// The constructor for the login model.
        /// </summary>
        /// <param name="signInManager">The sign in manager.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="userManager">The user manager.</param>
        /// <param name="emailSender">The email sender.</param>
        /// <param name="localizer">The localizer.</param>
        public LoginModel(ApplicationDbContext context, SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger, UserManager<ApplicationUser> userManager, IEmailSender emailSender, IStringLocalizer<LoginModel> localizer)
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
            _emailSender = emailSender;
            _localizer = localizer;
            _context = context;
        }

        /// <summary>
        /// The input model for the login page.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        /// The list of possible external logins. Only has Google.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        /// The return URL.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// The error message.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// The input model for the login page.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            /// The email.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            /// <summary>
            /// The password.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            /// The remember me option.
            /// </summary>
            [Display(Name = "Lembrar a minha palavra-passe")]
            public bool RememberMe { get; set; }
        }

        /// <summary>
        /// The get method for the login page.
        /// </summary>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns></returns>
        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        /// <summary>
        /// The method that's executed when the login is submitted.
        /// </summary>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns>If the model is invalid, it returns the page itself with errors. If its valid, redirects to the return url.</returns>
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                if (!Input.Email.Contains("@"))
                {
                    ModelState.AddModelError("EmailRed", _localizer["invalid_email"]);
                }
                else
                if (!Regex.IsMatch(Input.Password, Pattern))
                {
                    ModelState.AddModelError("PasswordRed", _localizer["invalid_password"]);
                }
                else {
                    var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);
                    if (result.Succeeded)
                    {
                        var foundUser = await _context.Users.Include(u => u.Bans).FirstOrDefaultAsync(u => u.Email == Input.Email);
                        if (foundUser.Bans != null) {
                            List<BanUser> sortedList = foundUser.Bans.OrderByDescending(u => u.EndsAt).ToList();
                            if (sortedList.Count >= 1 && sortedList[0].EndsAt.CompareTo(DateTime.Now) > 0) {
                                await _signInManager.SignOutAsync();
								return RedirectToAction("BanInfoWarning", "Home", new { date = sortedList[0].EndsAt, reason = sortedList[0].Reason });
							}
                        }
                        _logger.LogInformation("User logged in.");
                        return LocalRedirect(returnUrl);
                    }
                    if (result.RequiresTwoFactor)
                    {
                        return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                    }
                    if (result.IsLockedOut)
                    {
                        _logger.LogWarning("User account locked out.");
                        //return RedirectToPage("./Lockout");
                        ModelState.AddModelError("Danger", _localizer["blocked_account"] + " 10 " + _localizer["minutes"]);
                    } 
                    if (result.IsNotAllowed) 
                    {
                        var user = await _userManager.FindByEmailAsync(Input.Email);
                        if (user != null && !await _userManager.CheckPasswordAsync(user, Input.Password)) {
                            ModelState.AddModelError("Danger", _localizer["failed_login"]);
                        } else {
                            ModelState.AddModelError("Danger", _localizer["account_not_activated"]);
                            var userId = await _userManager.GetUserIdAsync(user);
                            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                            var callbackUrl = Url.Page(
                                "/Account/ConfirmEmail",
                                pageHandler: null,
                                values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                                protocol: Request.Scheme);
                            await _emailSender.SendEmailAsync(Input.Email, "TIDBEAT - " + _localizer["confirm_mail"],
                                $"{_localizer["please_confirm_link"]}, <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>{_localizer["clicking_here"]}</a>.");
                        }
                    } 
                    else
                    {
                        ModelState.AddModelError("Danger", _localizer["failed_login"]);
                    } 
                }
            }

            // If we got this far, something failed, redisplay form
            TempData["Email"] = Input.Email;
            TempData["Password"] = Input.Password;
            return Page();
        }
        /*
        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
            return Challenge(new AuthenticationProperties { RedirectUri = returnUrl }, "Google");
        }
        */
    }
}
