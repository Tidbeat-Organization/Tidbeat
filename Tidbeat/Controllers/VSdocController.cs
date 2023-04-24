using Microsoft.AspNetCore.Mvc;

namespace Tidbeat.Controllers {
    public class VSdocController : Controller {
        private readonly IWebHostEnvironment _env;

        public VSdocController(IWebHostEnvironment env) {
            _env = env;
        }

        public IActionResult Index() {
            /*
            var filePath = Path.Combine(_env.ContentRootPath, "VSdoc", "index.html");

            // Check if the file exists
            if (!System.IO.File.Exists(filePath)) {
                return NotFound();
            }

            return File(filePath, "text/html");
            */
            return View("topic_0000000000000259");
        }
    }
}
