using Tidbeat.Enums;
using Tidbeat.Models;

namespace Tidbeat.Services {
    public interface IRatingService { 
        Task<double> GetAverageRating(RatingType type, int postId);
        Task<bool> HasUserRated(RatingType type, int postId, int userId);
    }
}
