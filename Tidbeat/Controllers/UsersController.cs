using EllipticCurve.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tidbeat.Models;
using Tidbeat.Services;

namespace Tidbeat.Controllers
{
    /// <summary>
    /// The users controller used solely for the Index page and fetching/filtering users for a list.
    /// </summary>
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// The constructor for the UsersController.
        /// </summary>
        /// <param name="userManager">The user manager object for fetching the users from.</param>
        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// The Index method which returns a Page with a filtered list of users.
        /// </summary>
        /// <param name="name">The name of the user.</param>
        /// <param name="country">The country.</param>
        /// <param name="sort">The type of sorting.</param>
        /// <returns>The Index page.</returns>
        public async Task<IActionResult> Index(string name, string country, string sort)
        {
            var users = _userManager.Users
                .Where(u => u.FullName != "[deleted]")
                .ToList();
            ViewBag.TotalUsersCount = users.Count;

            // filter by name
            if (!string.IsNullOrEmpty(name))
            {
                name = name.ToLower();
                users = users.Where(u => u.FullName.ToLower().Contains(name)).ToList();
            }

            // store the search filters in ViewData so they can be used in the view
            ViewData["NameFilter"] = name;
            ViewData["CountryFilter"] = country;

            ViewData["OrderFilter"] = sort;

            // filter by country
            if (!string.IsNullOrEmpty(country))
            {
                country = country.ToLower();
                users = users.Where(u => u.Country != null && u.Country.ToLower().Contains(country)).ToList();
            }

            if (!string.IsNullOrEmpty(sort))
            {
                sort = sort.ToLower();
                switch (sort)
                {
                    case "a-z":
                        users = users.OrderBy(p => p.FullName).ToList();
                        break;
                    case "z-a":
                        users = users.OrderByDescending(p => p.FullName).ToList();
                        break;
                }
            }
            ViewBag.FilteredUsersCount = users.Count;

            ViewBag.Countries = GlobalizationService.CountryList().OrderBy(c => c).ToList();

            var currentuser = await _userManager.GetUserAsync(User);
            ViewBag.CurrentUser = currentuser;

            if (User != null && User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                var request = HttpContext.Request;
                var currentUrl = string.Format("{0}://{1}", request.Scheme, request.Host);
                TempData["Friends"] = await UtilityClass.SideBarAsync(user.Id, currentUrl);
            }
            
            return View(users.Take(9));
        }

        /// <summary>
        /// Fetches users based on filters and a offset. Used for a AJAX call in the Index page.
        /// </summary>
        /// <param name="name">The name filter of the users.</param>
        /// <param name="country">The country filter.</param>
        /// <param name="sort">The sorting parameter.</param>
        /// <param name="offset">The offset of the users it takes.</param>
        /// <returns>A partial view which is a list of filtered users.</returns>
        public async Task<IActionResult> getData(string name, string country, string sort, int offset = 0)
        {
            var users = await _userManager.Users
                .Where(u => u.FullName != "[deleted]")
                .ToListAsync();

            // filter by name
            if (!string.IsNullOrEmpty(name))
            {
                name = name.ToLower();
                users = users.Where(u => u.FullName.ToLower().Contains(name)).ToList();
            }
            ViewData["offset"] = offset;
            // filter by country
            if (!string.IsNullOrEmpty(country))
            {
                country = country.ToLower();
                users = users.Where(u => u.Country != null && u.Country.ToLower().Contains(country)).ToList();
            }

            if (!string.IsNullOrEmpty(sort))
            {
                sort = sort.ToLower();
                switch (sort)
                {
                    case "a-z":
                        users = users.OrderBy(p => p.FullName).ToList();
                        break;
                    case "z-a":
                        users = users.OrderByDescending(p => p.FullName).ToList();
                        break;
                }
            }

            return PartialView("_UserListPartial", users.Skip(offset).Take(9));
        }


    }
}