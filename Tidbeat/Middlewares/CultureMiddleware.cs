using Microsoft.Extensions.Options;
using System.Globalization;

namespace Tidbeat.Middlewares {
    public class CultureMiddleware {
        private readonly RequestDelegate _next;

        public CultureMiddleware(RequestDelegate next) {
            _next = next;
        }

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
