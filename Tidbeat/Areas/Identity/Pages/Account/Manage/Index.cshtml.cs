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
using Microsoft.Extensions.Localization;
using Tidbeat.Controllers;
using Tidbeat.Models;

namespace Tidbeat.Areas.Identity.Pages.Account.Manage
{
    /// <summary>
    /// The model class for the index page.
    /// </summary>
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IStringLocalizer<IndexModel> Localizer;

        /// <summary>
        /// The constructor for the index model.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="signInManager">The sign in manager.</param>
        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IStringLocalizer<IndexModel> localizer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            Localizer = localizer;
        }

        /// <summary>
        /// The username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The status message.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        /// The input model for the index page.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        /// The input model for the index page.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            /// The full name.
            /// </summary>
            [Required(ErrorMessage = "please_enter_a_valid_name")]
            [DataType(DataType.Text)]
            [MaxLength(30, ErrorMessage = "name_too_long")]
            [RegularExpression(@"^(?!_)[^<>""']*$", ErrorMessage = "name_may_not_contain")]
            [Display(Name = "Full name")]
            public string FullName { get; set; }

            /// <summary>
            /// The birthday date.
            /// </summary>
            [Required(ErrorMessage = "please_enter_a_valid_birthday_date")]
            [DataType(DataType.Date)]
            [Display(Name = "Birthday Date")]
            public DateTime BirthdayDate { get; set; }

            /// <summary>
            /// The user's gender.
            /// </summary>
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Gender")]
            public string Gender { get; set; }

            /// <summary>
            /// The user's about me.
            /// </summary>
            [DataType(DataType.Text)]
            [Display(Name = "About Me")]
            public string AboutMe { get; set; }

            /// <summary>
            /// The user's favorite genre.
            /// </summary>
            [Display(Name = "Favorite Genre")]
            public string FavoriteGenre { get; set; }

            /// <summary>
            /// The user's country.
            /// </summary>
            [Display(Name = "Country")]
            public string Country { get; set; }

        }

        /// <summary>
        /// The on load method. Loads the user's data.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
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

        /// <summary>
        /// The on get method. Loads the user's data and returns the page.
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
        /// The on post method. Updates the user's data and returns the page. Called when the page is submitted.
        /// </summary>
        /// <returns></returns>
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
            StatusMessage = Localizer["your_profile_has_been_updated"];
            return RedirectToPage();
        }
    }
}
