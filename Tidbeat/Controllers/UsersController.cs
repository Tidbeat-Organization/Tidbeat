using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tidbeat.Models;

namespace Tidbeat.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string name, string genre, string country)
        {
            var users = await _userManager.Users.ToListAsync();

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

            return View(users);

        }

    }
}