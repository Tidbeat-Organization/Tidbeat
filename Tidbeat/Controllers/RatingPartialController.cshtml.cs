using Microsoft.AspNetCore.Authorization;
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
        private readonly IRatingService _ratingService;
        private readonly UserManager<ApplicationUser> _userManager;

        public RatingPartialController(IRatingService ratingService, UserManager<ApplicationUser> userManager)
        {
            _ratingService = ratingService;
            _userManager = userManager;
        }

        public async Task<double> GetAverageRatings([FromQuery] RatingType type, [FromQuery] int id) {
            return await _ratingService.GetAverageRating(type, id);
        }

        public async Task<bool> HasUserRated([FromQuery] RatingType type, [FromQuery] int id) {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) {
                return false;
            }
            return await _ratingService.HasUserRated(type, id, user.Id);
        }

        public async Task<int> GetUserRate([FromQuery] RatingType type, [FromQuery] int id) {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) {
                return 0;
            }
            return await _ratingService.GetUserRate(type, id, user.Id);
        }
        [Authorize]
        public async Task SetUserRate([FromQuery] RatingType type, [FromQuery] int id, [FromQuery] int value) {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) {
                return;
            }
            await _ratingService.SetUserRate(type, id, user.Id, value);
        }
    }
}
