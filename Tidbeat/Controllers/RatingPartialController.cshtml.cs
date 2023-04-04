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
    /// <summary>
    /// Controls all the rating partials present in posts and comments.
    /// </summary>
    public class RatingPartialController : Controller
    {
        private readonly IRatingService _ratingService;
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Initializes needed services for the controller.
        /// </summary>
        /// <param name="ratingService">The service that provides rating functionality.</param>
        /// <param name="userManager">The user manager.</param>
        public RatingPartialController(IRatingService ratingService, UserManager<ApplicationUser> userManager)
        {
            _ratingService = ratingService;
            _userManager = userManager;
        }

        /// <summary>
        /// Gets the average rating of a post or comment.
        /// </summary>
        /// <param name="type">The type of the object to get the rating from.</param>
        /// <param name="id">The id of the object to get the rating from.</param>
        /// <remarks>GET: RatingPartial/GetAverageRating?type={value},int={value}</remarks>
        /// <returns>The average rating of the object.</returns>
        public async Task<double> GetAverageRatings([FromQuery] RatingType type, [FromQuery] int id) {
            return await _ratingService.GetAverageRating(type, id);
        }

        /// <summary>
        /// Gets if the current user has rated a post or comment.
        /// </summary>
        /// <param name="type">The type of the object to get the rating information from.</param>
        /// <param name="id">The id of the object to get the rating information from.</param>
        /// <remarks>GET: RatingPartial/GetNumberOfRatings?type={value},int={value}</remarks>
        /// <returns>A boolean about whether the current user has rated or not.</returns>
        public async Task<bool> HasUserRated([FromQuery] RatingType type, [FromQuery] int id) {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) {
                return false;
            }
            return await _ratingService.HasUserRated(type, id, user.Id);
        }

        /// <summary>
        /// Gets the value of the current user's rating of a post or comment.
        /// </summary>
        /// <param name="type">The type of the object to get the rating information from.</param>
        /// <param name="id">The id of the object to get the rating information from.</param>
        /// <remarks>GET: RatingPartial/GetUserRate?type={value},int={value}</remarks>
        /// <returns>The value of the current user's rating of the object.</returns>
        public async Task<int> GetUserRate([FromQuery] RatingType type, [FromQuery] int id) {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) {
                return 0;
            }
            return await _ratingService.GetUserRate(type, id, user.Id);
        }
        
        /// <summary>
        /// Sets the current user's rating of a post or comment.
        /// </summary>
        /// <param name="type">The type of the object to set the rating to.</param>
        /// <param name="id">The id of the object to set the rating to.</param>
        /// <param name="value">The value of the rating.</param>
        /// <remarks>GET: RatingPartial/SetUserRate?type={value},int={value},int={value}</remarks>
        /// <returns></returns>
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
