using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tidbeat.Data;
using Tidbeat.Models;
using Tidbeat.Services;

namespace Tidbeat.Controllers
{
    public class SidebarController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public SidebarController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }


        public async Task<IActionResult> GetSidebarData()
        {
            var currentuser = await _userManager.GetUserAsync(User);
            ViewBag.CurrentUser = currentuser;

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                var request = HttpContext.Request;
                var currentUrl = string.Format("{0}://{1}", request.Scheme, request.Host);
                TempData["Friends"] = await UtilityClass.SideBarAsync(user.Id, currentUrl);

                return View();
            }

            // return a default view or null
            return View("null");
        }


    }
}
