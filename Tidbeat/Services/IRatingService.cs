using Tidbeat.Models;

namespace Tidbeat.Services {
    public interface IRatingService { 
        Task<List<PostRating>> GetPostRatingsAsync(int postId);

    }
}
