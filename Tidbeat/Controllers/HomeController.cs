using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using Tidbeat.Models;

namespace Tidbeat.Controllers {
    /// <summary>
    /// Controls the Index (Main Page) and the Error404 page.
    /// </summary>
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager) {
            _logger = logger;
            _userManager = userManager;
        }
        /// <summary>
        /// Finds the Index view.
        /// </summary>
        /// <returns>Index view.</returns>
        public async Task<IActionResult> Index() {
            if (User.Identity.IsAuthenticated)
            {
                using (var client = new HttpClient())
                {
                    var user = await _userManager.GetUserAsync(User);
                    var request = HttpContext.Request;
                    var currentUrl = string.Format("{0}://{1}", request.Scheme, request.Host);
                    Console.WriteLine(currentUrl);
                    // Call the second action and get the JSON result
                    var response = await client.GetAsync(currentUrl+"/Follows/Followers?userId=" + user.Id);
                    response.EnsureSuccessStatusCode(); // Throws an exception if the status code is not 2xx

                    // Deserialize the JSON result and store it in TempData
                    var jsonResult = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<List<ApplicationUser>>(jsonResult);
                    TempData["Friends"] = data;
                }
                
 
            }
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
    }
}