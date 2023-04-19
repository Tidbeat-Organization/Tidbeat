// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Tidbeat.Controllers;
using Tidbeat.Models;

namespace Tidbeat.Areas.Identity.Pages.Account.Manage
{
    /// <summary>
    ///    The model class for the change password page.
    /// </summary>
    public class ChangePasswordModel : PageModel
    {
        public static string Pattern = RegisterModel.Pattern;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<ChangePasswordModel> _logger;
        private readonly IStringLocalizer<ChangePasswordModel> _localizer;

        /// <summary>
        /// The constructor for the change password model.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="signInManager">The sign in manager.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="localizer">The localizer.</param>
        public ChangePasswordModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<ChangePasswordModel> logger,
            IStringLocalizer<ChangePasswordModel> localizer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _localizer = localizer;
        }

        /// <summary>
        /// The input model for the change password page.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        /// The status message.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        /// The input model.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            /// The old password.
            /// </summary>
            [Required(ErrorMessage = "please_enter_your_password")]
            [DataType(DataType.Password)]
            [Display(Name = "current_password")]
            public string OldPassword { get; set; }

            /// <summary>
            /// The new password.
            /// </summary>
            [Required(ErrorMessage = "please_enter_a_password")]
            [StringLength(100, ErrorMessage = "the_0_must_be_at_least_2_at_max_1_characters", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "new_password")]
            public string NewPassword { get; set; }

            /// <summary>
            /// The confirm new password.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "confirm_new_password")]
            [Compare("NewPassword", ErrorMessage = "password_mismatch")]
            public string ConfirmPassword { get; set; }
        }

        /// <summary>
        /// The get method for the change password page.
        /// </summary>
        /// <returns>The page itself in case he has a password. If he doesn't have a password, redirects to SetPassword.</returns>
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToPage("./SetPassword");
            }

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
        /// The post method for the change password page.
        /// </summary>
        /// <returns>The page itself in case of an error. If the password is changed, redirects to the page itself.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (Input.NewPassword != Input.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", _localizer["password_mismatch"]);
                TempData["OldPassword"] = Input.OldPassword;
                TempData["NewPassword"] = Input.NewPassword;
                TempData["ConfirmPassword"] = Input.ConfirmPassword;
                return Page();
            }
            if (!Regex.IsMatch(Input.NewPassword, Pattern))
            {
                ModelState.AddModelError("NewPassword", _localizer["invalid_password"]);
                TempData["OldPassword"] = Input.OldPassword;
                TempData["NewPassword"] = Input.NewPassword;
                TempData["ConfirmPassword"] = Input.ConfirmPassword;
                return Page();
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            _userManager.PasswordValidators.Clear();
            _userManager.PasswordValidators.Add(new CustomPasswordValidator<ApplicationUser>());
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
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
                        if (error.Code == "PasswordMismatch")
                    {
                        ModelState.AddModelError("OldPassword", _localizer["password_mismatch_alt"]);
                    }
                    else
                        if (error.Code == "PasswordTooShort" || error.Code == "PasswordRequiresNonAlphanumeric" || error.Code == "PasswordRequiresDigit" || error.Code == "PasswordRequiresLower" || error.Code == "PasswordRequiresUpper")
                    {
                        ModelState.AddModelError("Danger", _localizer["invalid_password"]);
                    }
                    else
                    {
                        Console.WriteLine(error.Code);
                        ModelState.AddModelError("Danger", error.Description);
                    }
                }
                TempData["OldPassword"] = Input.OldPassword;
                TempData["NewPassword"] = Input.NewPassword;
                TempData["ConfirmPassword"] = Input.ConfirmPassword;
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation(_localizer["confirmation"]);
            StatusMessage = "Your password has been changed.";

            return RedirectToPage();
        }
    }
}
