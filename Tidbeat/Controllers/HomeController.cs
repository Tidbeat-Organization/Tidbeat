using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using Tidbeat.Models;

namespace Tidbeat.Controllers
{
    /// <summary>
    /// Controls the Index (Main Page) and the Error404 page.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }
        /// <summary>
        /// Finds the Index view.
        /// </summary>
        /// <returns>Index view.</returns>
        public async Task<IActionResult> Index()
        {
            var currentuser = await _userManager.GetUserAsync(User);
            ViewBag.CurrentUser = currentuser;

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                var request = HttpContext.Request;
                var currentUrl = string.Format("{0}://{1}", request.Scheme, request.Host);
                if (user != null) {
                    TempData["Friends"] = await UtilityClass.SideBarAsync(user.Id, currentUrl);
                }
            }
            ViewBag.RegisteredDailyUsers = await _userManager.Users.Where(u => u.CreationDate.HasValue && u.CreationDate.Value >= DateTime.Now.AddDays(-1)).CountAsync();
            
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        //Temporary for now
        public IActionResult Post()
        {
            return View();
        }

        public IActionResult Register(EnumRegisterPhase phase)
        {
            ViewBag.RegisterPhase = phase;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        /// <summary>
        /// Finds the 404 error page.
        /// </summary>
        /// <returns>404 error page.</returns>
        public IActionResult Error404()
        {
            return View();
        }
        /// <summary>
        /// Finds the Banning Info Warning page.
        /// </summary>
        /// <returns>Banning Info warning page.</returns>
        public IActionResult BanInfoWarning([FromQuery] DateTime date, string reason) {
            if (date.CompareTo(DateTime.Now) < 0) {
                return NotFound();
            }
			return View(Tuple.Create(date, reason));
		}
        /// <summary>
        /// Finds the Banning Info Warning page.
        /// </summary>
        /// <returns>Banning Info warning page.</returns>
        public IActionResult PrivilegesInfoWarning()
        {
            return View();
        }
    }
}