using Microsoft.Extensions.Options;
using System.Globalization;

namespace Tidbeat.Middlewares {
    /// <summary>
    /// The middleware that sets the culture of the application. It gets the culture from the cookies.
    /// </summary>
    public class CultureMiddleware {
        private readonly RequestDelegate _next;

        /// <summary>
        /// The constructor of the middleware.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        public CultureMiddleware(RequestDelegate next) {
            _next = next;
        }

        /// <summary>
        /// The method that is called when the middleware is invoked.
        /// </summary>
        /// <param name="context">The context of the request.</param>
        /// <param name="options">The options for the middleware.</param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context, IOptions<RequestLocalizationOptions> options) {
            string culture = context.Request.Cookies["culture"];
            if (!string.IsNullOrEmpty(culture)) {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
                CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(culture);
                CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(culture);
                CultureInfo.CurrentCulture = new CultureInfo(culture);
                CultureInfo.CurrentUICulture = new CultureInfo(culture);
            }

            await _next(context);
        }
    }
}
