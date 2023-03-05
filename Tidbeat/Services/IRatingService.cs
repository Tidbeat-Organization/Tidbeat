using Tidbeat.Enums;
using Tidbeat.Models;

namespace Tidbeat.Services {
    public interface IRatingService { 
        Task<double> GetAverageRating(RatingType type, int id);
        Task<bool> HasUserRated(RatingType type, int id, string userId);
    }
}
