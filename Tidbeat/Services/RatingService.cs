using Tidbeat.Data;
using Tidbeat.Models;

namespace Tidbeat.Services {
    public class RatingService : IRatingService {
        private readonly ApplicationDbContext _context;

        public RatingService(ApplicationDbContext context) {
            _context = context;
        }

        public Task<List<PostRating>> GetPostRatingsAsync(int postId) {
            throw new NotImplementedException();
        }
    }
}
