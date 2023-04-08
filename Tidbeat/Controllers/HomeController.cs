using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Tidbeat.Models;

namespace Tidbeat.Controllers {
    /// <summary>
    /// Controls the Index (Main Page) and the Error404 page.
    /// </summary>
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger) {
            _logger = logger;
        }
        /// <summary>
        /// Finds the Index view.
        /// </summary>
        /// <returns>Index view.</returns>
        public IActionResult Index() {
            return View();
        }

        public IActionResult Privacy() {
            return View();
        }

        //Temporary for now
        public IActionResult Post() {
            return View();
        }
        
        public IActionResult Register(EnumRegisterPhase phase)
        {
            ViewBag.RegisterPhase = phase;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
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
        public IActionResult BanInfoWarning()
        {
            return View();
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