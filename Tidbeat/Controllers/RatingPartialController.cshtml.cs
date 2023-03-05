using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Tidbeat.Data;
using Tidbeat.Enums;
using Tidbeat.Models;
using Tidbeat.Services;

namespace Tidbeat.Controllers
{
    public class RatingPartialController : Controller
    {
        IRatingService _ratingService;
        IServiceProvider _serviceProvider;

        public RatingPartialController(IRatingService ratingService, IServiceProvider serviceProvider)
        {
            _ratingService = ratingService;
            _serviceProvider = serviceProvider;
        }

        public async Task<double> GetAverageRatings([FromQuery] RatingType type, [FromQuery] int postId) {
            return await _ratingService.GetAverageRating(type, postId);
        }

        public async Task<bool> HasUserRated([FromQuery] RatingType type, [FromQuery] int postId) {
            var user = await _serviceProvider.GetRequiredService<UserManager<ApplicationUser>>().GetUserAsync(User);
            if (user == null) {
                return false;
            }
            return await _ratingService.HasUserRated(type, postId, user.Id);
        }
    }
}
