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
using NuGet.Protocol;
using Tidbeat.Models;
using System.Text.RegularExpressions;
namespace Tidbeat.Areas.Identity.Pages.Account
{
    
    public class RegisterModel : PageModel
    {
        private static string Pattern = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&_-])[A-Za-z\\d@$!%*?&_-]{6,}$";
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Nome completo")]
            public string FullName { get; set; }
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Date)]
            [Display(Name = "Data de Nascimento")]
            public DateTime BirthdayDate { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "Género")]
            public string Gender { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Palavra-Passe")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirmar palavra-passe")]
            [Compare("Password", ErrorMessage = "A palavra-passe e a confirmação de palavra-passe não são iguais.")]
            public string ConfirmPassword { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                if (!Input.Email.Contains("@"))
                {
                    ModelState.AddModelError("EmailRed", "O email é inválido");
                }
                else
                if (!Regex.IsMatch(Input.Password, Pattern))
                {
                    ModelState.AddModelError("PasswordRed", "A palavra-passe deve conter, pelo menos 6 caracteres, dos quais têm que ter um número [0-9],uma letra minuscula [a-z], uma letra maiuscula [A-Z] e um caracter especial [@&?%]");
                }
                else
                if (Input.Password != Input.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPasswordRed", "A palavra-passe e a confirmação de palavra-passe não são iguais.");
                }
                else
                if (String.IsNullOrEmpty(Input.FullName))
                {
                    ModelState.AddModelError("NameRed", "Não um nome válido");
                } else
                if (Input.Gender != "Masculino" && Input.Gender != "Feminino" && Input.Gender != "Não Binário")
                {
                    ModelState.AddModelError("GenderRed", "Não é um género válido");
                }else
                if( DateTime.Compare(Input.BirthdayDate.AddYears(13),DateTime.Now) > 0)
                {
                    ModelState.AddModelError("AgeRed", "Para criar a Conta têm que ter 13 anos");
                }
                else
                {
                    var user = CreateUser();
                    user.FullName = Input.FullName;
                    user.BirthdayDate = Input.BirthdayDate;
                    user.Gender = Input.Gender;

                    await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                    await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                    var result = await _userManager.CreateAsync(user, Input.Password);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created a new account with password.");

                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                            protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(Input.Email, "TIDBEAT - Confirmar o teu mail",
                            $"Por favor, confirma a tua conta através do link, <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicando aqui</a>.");

                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                        }
                        else
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
                    }
                    foreach (var error in result.Errors)
                    {

                        if (error.Code == "DuplicateUserName")
                        {
                            ModelState.AddModelError("EmailRed", "O email já se encontra registado");
                        } else
                        if (error.Code == "DefaultError") 
                        {
                            ModelState.AddModelError("Danger", "Erro: Ocorreu um erro, por favor tente mais tarde");
                        } else
                        if (error.Code == "ConcurrencyFailure") {
                            ModelState.AddModelError("Danger", "Erro: Multiplas pessoas estão a modificar a conta");
                        }
                        else
                        if (error.Code == "InvalidEmail")
                        {
                            ModelState.AddModelError("EmailRed", "O email é inválido");
                        }
                        else
                        if (error.Code == "PasswordMismatch")
                        {
                            ModelState.AddModelError("ConfirmPasswordRed", "A palavra-passe e a confirmação de palavra-passe não são iguais.");
                        }
                        else
                        if (error.Code == "PasswordTooShort" || error.Code == "PasswordRequiresNonAlphanumeric" || error.Code == "PasswordRequiresDigit" || error.Code == "PasswordRequiresLower" || error.Code == "PasswordRequiresUpper")
                        {
                            ModelState.AddModelError("PasswordRed", "A palavra-passe deve conter, pelo menos 6 caracteres, dos quais têm que ter um número [0-9],uma letra minuscula [a-z], uma letra maiuscula [A-Z] e um caracter especial [@&?%]");
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
