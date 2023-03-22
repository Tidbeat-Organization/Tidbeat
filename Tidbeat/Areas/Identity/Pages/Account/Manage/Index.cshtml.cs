// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tidbeat.Models;

namespace Tidbeat.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

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
            [Required(ErrorMessage = "please_enter_a_valid_name")]
            [DataType(DataType.Text)]
            [Display(Name = "Full name")]
            public string FullName { get; set; }

            [Required(ErrorMessage = "please_enter_a_valid_birthday_date")]
            [DataType(DataType.Date)]
            [Display(Name = "Birthday Date")]
            public DateTime BirthdayDate { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Gender")]
            public string Gender { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "About Me")]
            public string AboutMe { get; set; }

            [Display(Name = "Favorite Genre")]
            public string FavoriteGenre { get; set; }

            [Display(Name = "Country")]
            public string Country { get; set; }

        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            Input = new InputModel {
                FullName = currentUser.FullName,
                BirthdayDate = currentUser.BirthdayDate,
                Gender = currentUser.Gender,
                AboutMe = currentUser.AboutMe,
                FavoriteGenre = currentUser.FavoriteGenre,
                Country = currentUser.Country
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
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

            user.FullName = Input.FullName;
            user.BirthdayDate = Input.BirthdayDate;
            user.Gender = Input.Gender;
            user.AboutMe = Input.AboutMe;
            user.FavoriteGenre = Input.FavoriteGenre;
            user.Country = Input.Country;
            await _userManager.UpdateAsync(user);

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
