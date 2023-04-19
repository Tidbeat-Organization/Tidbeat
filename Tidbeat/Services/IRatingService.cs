using Tidbeat.Enums;
using Tidbeat.Models;

namespace Tidbeat.Services {
    /// <summary>
    /// It takes care of all operations related to ratings.
    /// </summary>
    public interface IRatingService {
        /// <summary>
        /// Gets the average rating of a post or comment.
        /// </summary>
        /// <param name="type">The type of the object to get the rating from.</param>
        /// <param name="id">The id of the object to get the rating from.</param>
        /// <returns>The average rating of the object.</returns>
        Task<double> GetAverageRating(RatingType type, int id);

        /// <summary>
        /// Gets whether the user has rated or not.
        /// </summary>
        /// <param name="type">The type of the object to get the rating from.</param>
        /// <param name="id">The id of the object to get the rating from.</param>
        /// <param name="userId">The id of the user to check if he rated or not.</param>
        /// <returns>True if the user has rated, false otherwise.</returns>
        Task<bool> HasUserRated(RatingType type, int id, string userId);

        /// <summary>
        /// Gets the user's rating. If the user hasn't rated, it returns 0.
        /// </summary>
        /// <param name="type">The type of the object to get the rating from.</param>
        /// <param name="id">The id of the object to get the rating from.</param>
        /// <param name="userId">The id of the user to get the rating from.</param>
        /// <returns>The user's rating.</returns>
        Task<int> GetUserRate(RatingType type, int id, string userId);

        /// <summary>
        /// Sets the user's rating.
        /// </summary>
        /// <param name="type">The type of the object to get the rating from.</param>
        /// <param name="id">The id of the object to get the rating from.</param>
        /// <param name="userId">The id of the user to set the rating from.</param>
        /// <param name="value">The value of the rating.</param>
        /// <returns></returns>
        Task SetUserRate(RatingType type, int id, string userId, int value);
    }
}
