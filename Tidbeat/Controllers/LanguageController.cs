using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Tidbeat.Controllers {
    public class LanguageController : Controller {

        [HttpPost]
        public IActionResult SetLanguage(string language) {
            CultureInfo.CurrentCulture = new CultureInfo(language);
            CultureInfo.CurrentUICulture = new CultureInfo(language);
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(language)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });
            return Redirect("/");
        }
    }
}
