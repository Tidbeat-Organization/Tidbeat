// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;
using NuGet.Protocol;
using Tidbeat.Models;
using System.Text.RegularExpressions;
namespace Tidbeat.Areas.Identity.Pages.Account
{
    /// <summary>
    /// The model class for the register page.
    /// </summary>
    public class RegisterModel : PageModel
    {
        public static string Pattern = "^(?!_)(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?!.*[<>\\'\\\"])[^\\x3C\\x3E\\x27\\x22]{6,}$";
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IStringLocalizer<RegisterModel> _localizer;

        /// <summary>
        /// The constructor for the register model.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="userStore">The user store.</param>
        /// <param name="signInManager">The sign in manager.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="emailSender">The email sender.</param>
        /// <param name="localizer">The localizer.</param>
        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IStringLocalizer<RegisterModel> localizer)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _localizer = localizer;
        }

        /// <summary>
        /// The input model for the register page.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        /// The return url for the register page.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// The list of external logins for the register page.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        /// The method that is called when the page is loaded.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            /// The full name for the register page.
            /// </summary>
            [Required]
            [DataType(DataType.Text)]
            [MaxLength(30, ErrorMessage = "name_too_long")]
            [Display(Name = "Nome apresentado")]
            public string FullName { get; set; }
            /// <summary>
            /// The email for the register page.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            /// The birthday date for the register page.
            /// </summary>
            [Required]
            [DataType(DataType.Date)]
            [Display(Name = "Data de Nascimento")]
            public DateTime BirthdayDate { get; set; }

            /// <summary>
            /// The user's gender for the register page.
            /// </summary>
            [DataType(DataType.Text)]
            [Display(Name = "Género")]
            public string Gender { get; set; }

            /// <summary>
            /// The password for the register page.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Palavra-Passe")]
            public string Password { get; set; }

            /// <summary>
            /// The confirmation password for the register page.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirmar palavra-passe")]
            [Compare("Password", ErrorMessage = "A palavra-passe e a confirmação de palavra-passe não são iguais.")]
            public string ConfirmPassword { get; set; }
        }

        /// <summary>
        /// The method that is called when the page is loaded.
        /// </summary>
        /// <param name="returnUrl">The return url.</param>
        /// <returns></returns>
        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        /// <summary>
        /// The method that is called when the register is submitted.
        /// </summary>
        /// <param name="returnUrl">The return url.</param>
        /// <returns>Returns the page itself with errors if the model isn't valid. If its valid, sends an email and redirects to the register confirmation.</returns>
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
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
                if (String.IsNullOrEmpty(Input.FullName))
                {
                    ModelState.AddModelError("NameRed", _localizer["invalid_name"]);
                } else if (Input.FullName.Contains('<') || Input.FullName.Contains('>') || Input.FullName.StartsWith('_') || Input.FullName.Contains('"') || Input.FullName.Contains("'")) {
                    ModelState.AddModelError("NameRed", _localizer["name_may_not_contain"]);
                }
                else
                if (Input.Gender != "male" && Input.Gender != "female" && Input.Gender != "non_binary")
                {
                    ModelState.AddModelError("GenderRed", _localizer["invalid_gender"]);
                }else
                if( DateTime.Compare(Input.BirthdayDate.AddYears(13),DateTime.Now) > 0)
                {
                    ModelState.AddModelError("AgeRed", _localizer["invalid_birthday_date"]);
                }
                else
                {
                    var user = CreateUser();
                    user.FullName = Input.FullName;
                    user.BirthdayDate = Input.BirthdayDate;
                    user.Gender = Input.Gender;
                    user.CreationDate = DateTime.Now;

                    await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                    await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                    _userManager.PasswordValidators.Clear();
                    _userManager.PasswordValidators.Add(new CustomPasswordValidator<ApplicationUser>());
                    var result = await _userManager.CreateAsync(user, Input.Password);
                    

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "NormalUser");
                        _logger.LogInformation("User created a new account with password.");

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

                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                        }
                        else
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                        }
                    }
                    foreach (var error in result.Errors)
                    {

                        if (error.Code == "DuplicateUserName")
                        {
                            ModelState.AddModelError("EmailRed", _localizer["email_already_exists"]);
                        } else
                        if (error.Code == "DefaultError") 
                        {
                            ModelState.AddModelError("Danger", _localizer["default_error"]);
                        } else
                        if (error.Code == "ConcurrencyFailure") {
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
            }
            TempData["Email"] = Input.Email;
            TempData["Password"] = Input.Password;
            TempData["ConfirmPassword"] = Input.ConfirmPassword;
            TempData["Date"] = Input.BirthdayDate.ToString("yyyy-MM-dd");
            TempData["Name"] = Input.FullName; 
            TempData["Gender"] = Input.Gender;
            // If we got this far, something failed, redisplay form
            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
