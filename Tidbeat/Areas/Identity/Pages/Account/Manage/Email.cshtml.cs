// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Localization;
using Tidbeat.Controllers;
using Tidbeat.Models;

namespace Tidbeat.Areas.Identity.Pages.Account.Manage
{
    /// <summary>
    ///    The model class for the email page.
    /// </summary>
    public class EmailModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IStringLocalizer<EmailModel> _localizer;

        /// <summary>
        /// The constructor for the email model.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="signInManager">The sign in manager.</param>
        /// <param name="emailSender">The email sender.</param>
        /// <param name="localizer">The localizer.</param>
        public EmailModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender, 
            IStringLocalizer<EmailModel> localizer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _localizer = localizer;
        }

        /// <summary>
        /// The email address.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The confirmation status.
        /// </summary>
        public bool IsEmailConfirmed { get; set; }

        /// <summary>
        /// The status message.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        /// The input model for the email page.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        /// The input model for the email page.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            /// The new email address.
            /// </summary>
            [Required(ErrorMessage = "please_enter_a_valid_email_address")]
            [EmailAddress(ErrorMessage = "invalid_email_address_format")]
            [Display(Name = "New email")]
            public string NewEmail { get; set; }

            /// <summary>
            /// The new password.
            /// </summary>
            [AllowNull]
            public string NewPassword { get; set; }

            /// <summary>
            /// The confirmation of the new password.
            /// </summary>
            [AllowNull]
            public string ConfirmNewPassword { get; set; }
        }

        /// <summary>
        /// The load method. Loads the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        private async Task LoadAsync(ApplicationUser user)
        {
            var email = await _userManager.GetEmailAsync(user);
            Email = email;

            Input = new InputModel
            {
                NewEmail = email,
            };

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
        }

        /// <summary>
        /// The get method. Loads the user and returns the page.
        /// </summary>
        /// <returns>The page.</returns>
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);

            if (User != null && User.Identity.IsAuthenticated)
            {
                var userr = await _userManager.GetUserAsync(User);
                var request = HttpContext.Request;
                var currentUrl = string.Format("{0}://{1}", request.Scheme, request.Host);
                TempData["Friends"] = await UtilityClass.SideBarAsync(userr.Id, currentUrl);
            }

            return Page();
        }

        /// <summary>
        /// The post method. Changes the email address.
        /// </summary>
        /// <returns>If something is invalid, returns the page with errors. If its ok, redirects to the same page with the operation done.</returns>
        public async Task<IActionResult> OnPostChangeEmailAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid) {
                await LoadAsync(user);
            }
            else {
                var email = await _userManager.GetEmailAsync(user);
                if (Input.NewEmail != email) {
                    if (user.PasswordHash == null) {
                        if (Input.NewPassword == null) {
                            ModelState.AddModelError("NewPassword", _localizer["you_must_enter_password_to_change_email"]);
                            await LoadAsync(user);
                            return Page();
                        }
                        else
                        if (!Regex.IsMatch(Input.NewPassword, RegisterModel.Pattern)) {
                            ModelState.AddModelError("NewPassword", _localizer["password_must_contain_at_least"]);
                            await LoadAsync(user);
                            return Page();
                        }
                        else if (Input.NewPassword != Input.ConfirmNewPassword) {
                            ModelState.AddModelError("ConfirmNewPassword", _localizer["password_mismatch"]);
                            await LoadAsync(user);
                            return Page();
                        }
                    }
                    var foundUser = await _userManager.FindByEmailAsync(Input.NewEmail);
                    if (foundUser != null) {
                        ModelState.AddModelError("NewEmail", _localizer["email_already_taken"]);
                        await LoadAsync(user);
                        return Page();
                    }
                    await _userManager.AddPasswordAsync(user, Input.NewPassword);
                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmailChange",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, email = Input.NewEmail, code = code },
                        protocol: Request.Scheme);
                    await _emailSender.SendEmailAsync(
                        Input.NewEmail,
                        _localizer["confirm_mail"],
                        $"{_localizer["please_confirm_link"]} <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>{_localizer["clicking_here"]}</a>.");

                    StatusMessage = _localizer["confirmation_link_sent"];
                    return RedirectToPage();
                }
            }

            StatusMessage = _localizer["email_unchanged"];
            return RedirectToPage();
        }

        /// <summary>
        /// The post method. Sends the verification email.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = userId, code = code },
                protocol: Request.Scheme);
            await _emailSender.SendEmailAsync(
                email,
                _localizer["confirm_mail"],
                $"{_localizer["please_confirm_link"]} <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>{_localizer["clicking_here"]}</a>.");

            StatusMessage = _localizer["verification_email_sent"];
            return RedirectToPage();
        }
    }
}
