using Microsoft.AspNetCore.Mvc;

namespace Tidbeat.Controllers {
    public class VSdocController : Controller {
        private readonly IWebHostEnvironment _env;

        public VSdocController(IWebHostEnvironment env) {
            _env = env;
        }

        public IActionResult Index() {
            return View("topic_0000000000000259");
        }
    }
}
