// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Localization;
using Tidbeat.Models;

namespace Tidbeat.Areas.Identity.Pages.Account
{
    /// <summary>
    /// The model class for the forgot password page.
    /// </summary>
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IStringLocalizer<ForgotPasswordModel> _localizer;

        /// <summary>
        /// The constructor for the forgot password model.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="emailSender">The email sender.</param>
        /// <param name="localizer">The localizer.</param>
        public ForgotPasswordModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender, IStringLocalizer<ForgotPasswordModel> localizer)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _localizer = localizer;
        }

        /// <summary>
        /// The input model for the forgot password page.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        /// The input model for the forgot password page.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            /// The email address.
            /// </summary>
            [EmailAddress]
            public string Email { get; set; }
        }

        /// <summary>
        /// The method that's called when the forgot password is submitted.
        /// </summary>
        /// <returns>If the model is invalid, returns the page. If its valid, sends an email and redirects to the forgot password confirmation.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(Input.Email)) {
                ModelState.AddModelError("Email", _localizer["invalid_email"].Value);
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(
                    Input.Email,
                    "TIDBEAT - " + _localizer["email_reset_password"],
                    $"{_localizer["email_please_reset"]} <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>{_localizer["clicking_here"]}</a>.");

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}
