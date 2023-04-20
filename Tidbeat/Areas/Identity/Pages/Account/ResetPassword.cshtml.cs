// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using Microsoft.Extensions.Localization;
using Tidbeat.Models;

namespace Tidbeat.Areas.Identity.Pages.Account
{
    /// <summary>
    /// The model class for the reset password page.
    /// </summary>
    public class ResetPasswordModel : PageModel
    {
        private static string Pattern = RegisterModel.Pattern;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStringLocalizer<ResetPasswordModel> _localizer;

        /// <summary>
        /// The constructor for the reset password model.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="localizer">The localizer.</param>
        public ResetPasswordModel(UserManager<ApplicationUser> userManager, IStringLocalizer<ResetPasswordModel> localizer)
        {
            _userManager = userManager;
            _localizer = localizer;
        }

        /// <summary>
        /// The input model for the reset password page.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        /// The input model class.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            /// The email address.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            /// <summary>
            /// The password.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            /// The confirmation password.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            /// <summary>
            /// The code.
            /// </summary>
            [Required]
            public string Code { get; set; }

        }

        /// <summary>
        /// The get method for the reset password page.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>A bad request in case its null or a page.</returns>
        public IActionResult OnGet(string code = null)
        {
            if (code == null)
            {
                return BadRequest("A code must be supplied for password reset.");
            }
            else
            {
                Input = new InputModel
                {
                    Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
                };
                return Page();
            }
        }

        /// <summary>
        /// The post method for the reset password page.
        /// </summary>
        /// <returns>Returns the page itself with errors in case anything isn't correct. If its correct, redirects to the reset password confirmation.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!Input.Email.Contains("@"))
            {
                ModelState.AddModelError("EmailRed", _localizer["invalid_email"]);
            }
            else
                if (!Regex.IsMatch(Input.Password, Pattern))
            {
                ModelState.AddModelError("PasswordRed", _localizer["invalid_password"]);
            }
            else
                if (Input.Password != Input.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPasswordRed", _localizer["password_mismatch"]);
            }
            else
            {

                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist
                    return RedirectToPage("./ResetPasswordConfirmation");
                }
                _userManager.PasswordValidators.Clear();
                _userManager.PasswordValidators.Add(new CustomPasswordValidator<ApplicationUser>());
                var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
                if (result.Succeeded)
                {
                    return RedirectToPage("./ResetPasswordConfirmation");
                }

                foreach (var error in result.Errors)
                {

                    if (error.Code == "DefaultError")
                    {
                        ModelState.AddModelError("Danger", _localizer["default_error"]);
                    }
                    else
                            if (error.Code == "ConcurrencyFailure")
                    {
                        ModelState.AddModelError("Danger", _localizer["concurrency_failure"]);
                    }
                    else
                            if (error.Code == "InvalidEmail")
                    {
                        ModelState.AddModelError("EmailRed", _localizer["invalid_email"]);
                    }
                    else
                            if (error.Code == "PasswordMismatch")
                    {
                        ModelState.AddModelError("ConfirmPasswordRed", _localizer["password_mismatch"]);
                    }
                    else
                            if (error.Code == "PasswordTooShort" || error.Code == "PasswordRequiresNonAlphanumeric" || error.Code == "PasswordRequiresDigit" || error.Code == "PasswordRequiresLower" || error.Code == "PasswordRequiresUpper")
                    {
                        ModelState.AddModelError("PasswordRed", _localizer["invalid_password"]);
                    }
                    else
                    {
                        Console.WriteLine(error.Code);
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            TempData["Email"] = Input.Email;
            TempData["Password"] = Input.Password;
            TempData["ConfirmPassword"] = Input.ConfirmPassword;
            return Page();
        }
    }
}
