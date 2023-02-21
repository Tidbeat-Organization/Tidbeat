using Microsoft.AspNetCore.Mvc;

namespace Tidbeat.Controllers
{
    public class MusicsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
