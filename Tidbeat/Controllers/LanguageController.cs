using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Tidbeat.Controllers {
    /// <summary>
    /// Controls the localization aspect of the website.
    /// </summary>
    public class LanguageController : Controller {
        /// <summary>
        /// Changes the current language of the website.
        /// </summary>
        /// <param name="language">The language you want to change to. It should be like: "en_US" or "pt_PT"</param>
        /// <returns>A redirect to the Main Page.</returns>
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
