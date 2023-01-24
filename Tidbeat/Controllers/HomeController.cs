using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Tidbeat.Models;

namespace Tidbeat.Controllers {
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger) {
            _logger = logger;
        }

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

        [HttpPost]
        public IActionResult Register(EnumRegisterPhase phase)
        {
            ViewBag.RegisterPhase = phase;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}