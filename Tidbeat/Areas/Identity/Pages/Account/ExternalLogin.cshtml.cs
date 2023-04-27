// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable


using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Tidbeat.Models;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Localization;
using Tidbeat.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Tidbeat.Areas.Identity.Pages.Account
{
    /// <summary>
    ///    The model class for the external login page. This page shows up when the user finishes the external login in the provider.
    /// </summary>
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ExternalLoginModel> _logger;
        private readonly IStringLocalizer<ExternalLoginModel> _localizer;
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// The constructor for the external login model.
        /// </summary>
        /// <param name="signInManager">The sign in manager.</param>
        /// <param name="userManager">The user manager.</param>
        /// <param name="userStore">The user store.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="emailSender">The email sender.</param>
        /// <param name="localizer">The localizer.</param>
        public ExternalLoginModel(
            ApplicationDbContext context,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            ILogger<ExternalLoginModel> logger,
            IEmailSender emailSender,
            IStringLocalizer<ExternalLoginModel> localizer)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _logger = logger;
            _emailSender = emailSender;
            _localizer = localizer;
        }

        /// <summary>
        /// The input model for the external login page.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        /// The name of the external login provider: in our case, its Google.
        /// </summary>
        public string ProviderDisplayName { get; set; }

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
        /// The input model for the external login page.
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
            /// The full name.
            /// </summary>
            [MaxLength(30, ErrorMessage = "name_too_long")]
            [Required(ErrorMessage = "name_required")]
            public string FullName { get; set; }

            /// <summary>
            /// The birthday date.
            /// </summary>
            [Required(ErrorMessage = "birthday_date_required")]
            [DataType(DataType.Date)]
            public DateTime BirthdayDate { get; set; }

            /// <summary>
            /// The user's gender.
            /// </summary>
            [Required(ErrorMessage = "gender_required")]
            public string Gender { get; set; }
        }
        
        /// <summary>
        /// The method that is called when the page is loaded.
        /// </summary>
        /// <param name="returnUrl">The return URL.</param>
        public IActionResult OnGet() => RedirectToPage("./Login");

        /// <summary>
        /// The method that is called when the user clicks on the external login button.
        /// </summary>
        /// <param name="provider">The name of the external login provider.</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns></returns>
        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        /// <summary>
        /// The method that is called when the user finishes the external login in the provider.
        /// </summary>
        /// <param name="returnUrl">The return URL.</param>
        /// <param name="remoteError">The error message.</param>
        /// <returns>If there are issues, it returns to the Login page. If not, it continues to the ExternalLogin page.</returns>
        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }
            //var foundUser = await _userManager.FindByEmailAsync(info.Principal.FindFirstValue(ClaimTypes.Email));
            var foundUser = await _context.Users.Include(u => u.Bans).FirstOrDefaultAsync(u => u.Email == info.Principal.FindFirstValue(ClaimTypes.Email));
            
            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded || foundUser != null)
            {
                ApplicationUser userCheck;
                if (foundUser == null) {
                    var userResult = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                    userCheck = await _context.Users.Include(u => u.Bans).FirstOrDefaultAsync(u => userResult.Id == u.Id);
                } else {
                    userCheck = foundUser;
                }
                if (userCheck.Bans != null)
                {
                    List<BanUser> sortedList = userCheck.Bans.OrderByDescending(u => u.EndsAt).ToList();
                    if (sortedList.Count >= 1 && sortedList[0].EndsAt.CompareTo(DateTime.Now) > 0)
                    {
                        //Page for tempBan
                        await _signInManager.SignOutAsync();
                        return RedirectToAction("BanInfoWarning", "Home", new { date = sortedList[0].EndsAt, reason = sortedList[0].Reason }); //Change the view when its done
                    }
                }
                if (foundUser != null) {
                    await _signInManager.SignInAsync(foundUser, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ReturnUrl = returnUrl;
                ProviderDisplayName = info.ProviderDisplayName;
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    Input = new InputModel
                    {
                        Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                    };
                }
                return Page();
            }
        }

        /// <summary>
        /// The method that is called when the user clicks on the "Confirm" button.
        /// </summary>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns>If there are issues, it returns to the ExternalLogin page. If not, it finishes the login process.</returns>
        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            // Get the information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information during confirmation.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            if (ModelState.IsValid)
            {
                if (String.IsNullOrEmpty(Input.FullName))
                {
                    ModelState.AddModelError(string.Empty, _localizer["invalid_name"]);
                }
                else
                if (Input.Gender != "male" && Input.Gender != "female" && Input.Gender != "non_binary")
                {
                    ModelState.AddModelError(string.Empty, _localizer["invalid_gender"]);
                }
                else
                if (DateTime.Compare(Input.BirthdayDate.AddYears(13), DateTime.Now) > 0)
                {
                    ModelState.AddModelError(string.Empty, _localizer["invalid_birthday_date"]);
                }
                else
                {
                    var user = CreateUser();
                    await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                    await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                    user.FullName = Input.FullName;
                    user.BirthdayDate = Input.BirthdayDate;
                    user.Gender = Input.Gender;
                    user.EmailConfirmed = true;
                    user.CreationDate = DateTime.Now;

                    var result = await _userManager.CreateAsync(user);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "NormalUser");
                        result = await _userManager.AddLoginAsync(user, info);
                        if (result.Succeeded)
                        {
                            _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                            await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
                            return LocalRedirect(returnUrl);
                        }
                    }
                    foreach (var error in result.Errors)
                    {
                        if (error.Code == "DuplicateUserName")
                        {
                            ModelState.AddModelError(string.Empty, _localizer["email_already_exists"]);
                        }
                        else
                        if (error.Code == "DefaultError")
                        {
                            ModelState.AddModelError(string.Empty, _localizer["default_error"]);
                        }
                        else
                        if (error.Code == "ConcurrencyFailure")
                        {
                            ModelState.AddModelError(string.Empty, _localizer["concurrency_failure"]);
                        }
                        else
                        if (error.Code == "InvalidEmail")
                        {
                            ModelState.AddModelError(string.Empty, _localizer["invalid_email"]);
                        }
                        else
                        {
                            Console.WriteLine(error.Code);
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }

            ProviderDisplayName = info.ProviderDisplayName;
            ReturnUrl = returnUrl;
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
                    $"override the external login page in /Areas/Identity/Pages/Account/ExternalLogin.cshtml");
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
