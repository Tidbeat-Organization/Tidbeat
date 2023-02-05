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
using Tidbeat.Models;

namespace Tidbeat.Areas.Identity.Pages.Account
{
    public class ResetPasswordModel : PageModel
    {
        private static string Pattern = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&_-])[A-Za-z\\d@$!%*?&_-]{6,}$";
        private readonly UserManager<ApplicationUser> _userManager;

        public ResetPasswordModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
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
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            public string Code { get; set; }

        }

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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

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
            {

                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist
                    return RedirectToPage("./ResetPasswordConfirmation");
                }

                var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
                if (result.Succeeded)
                {
                    return RedirectToPage("./ResetPasswordConfirmation");
                }

                foreach (var error in result.Errors)
                {

                    if (error.Code == "DefaultError")
                    {
                        ModelState.AddModelError("Danger", "Erro: Ocorreu um erro, por favor tente, mais tarde");
                    }
                    else
                            if (error.Code == "ConcurrencyFailure")
                    {
                        ModelState.AddModelError("Danger", "Erro: Multiplas, pessoas estão a modificar a conta");
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
            TempData["Email"] = Input.Email;
            TempData["Password"] = Input.Password;
            TempData["ConfirmPassword"] = Input.ConfirmPassword;
            return Page();
        }
    }
}
