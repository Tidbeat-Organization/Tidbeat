using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tidbeat.Models;
using Tidbeat.Services;

namespace Tidbeat.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string name, string country)
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

            // store the search filters in ViewData so they can be used in the view
            ViewData["NameFilter"] = name;
            ViewData["CountryFilter"] = country;

            // filter by country
            if (!string.IsNullOrEmpty(country))
            {
                country = country.ToLower();
                users = users.Where(u => u.Country != null && u.Country.ToLower().Contains(country)).ToList();
            }

            ViewBag.Countries = GlobalizationService.CountryList().OrderBy(c => c).ToList();

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                var request = HttpContext.Request;
                var currentUrl = string.Format("{0}://{1}", request.Scheme, request.Host);
                TempData["Friends"] = await UtilityClass.SideBarAsync(user.Id, currentUrl);
            }
            return View(users.Take(9));
        }

        public async Task<IActionResult> getData(string name, string country, int offset = 0)
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

            // filter by country
            if (!string.IsNullOrEmpty(country))
            {
                country = country.ToLower();
                users = users.Where(u => u.Country != null && u.Country.ToLower().Contains(country)).ToList();
            }
            return PartialView("_UserListPartial", users.Skip(offset).Take(9));
        }


    }
}