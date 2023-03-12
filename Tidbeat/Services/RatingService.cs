using Microsoft.EntityFrameworkCore;
using Tidbeat.Data;
using Tidbeat.Enums;
using Tidbeat.Models;

namespace Tidbeat.Services {
    public class RatingService : IRatingService {
        private readonly ApplicationDbContext _context;

        public RatingService(ApplicationDbContext context) {
            _context = context;
        }

        public async Task<double> GetAverageRating(RatingType type, int id) {
            switch (type) {
                case RatingType.Post:
                    double? average = await _context.PostRatings.Where(r => r.Post.PostId == id).AverageAsync(r => r.Value);
                    return average ?? 0;
                case RatingType.Comment:
                    throw new NotImplementedException("Comment rating type has not been implemented");
                default:
                    throw new ArgumentException("Invalid rating type. Make sure you added the type to the switch statement");
            }
        }

        public async Task<bool> HasUserRated(RatingType type, int id, string userId) {
            switch (type) {
                case RatingType.Post:
                    return await _context.PostRatings.Where(r => r.Post.PostId == id && r.User.Id == userId).AnyAsync(r => r.Value > 0);
                case RatingType.Comment:
                    throw new NotImplementedException("Comment rating type has not been implemented");
                default:
                    throw new ArgumentException("Invalid rating type. Make sure you added the type to the switch statement");
            }
        }

        public async Task<int> GetUserRate(RatingType type, int id, string userId) {
            switch (type) {
                case RatingType.Post:
                    var rating = await _context.PostRatings.FirstOrDefaultAsync(r => r.Post.PostId == id && r.User.Id == userId);
                    if (rating == null) return 0;
                    return rating.Value;
                case RatingType.Comment:
                    throw new NotImplementedException("Comment rating type has not been implemented");
                default:
                    throw new ArgumentException("Invalid rating type. Make sure you added the type to the switch statement");
            }
        }

        public async Task SetUserRate(RatingType type, int id, string userId, int value) {
            switch (type) {
                case RatingType.Post:
                    var rating = await _context.PostRatings.FirstOrDefaultAsync(r => r.Post.PostId == id && r.User.Id == userId);
                    if (rating != null) {
                        rating.Value = value;
                    } else { 
                        var postRating = new PostRating() {
                            Post = await _context.Posts.FindAsync(id),
                            User = await _context.Users.FindAsync(userId),
                            Value = value
                        };
                        _context.PostRatings.Add(postRating);
                    }
                    await _context.SaveChangesAsync();
                    break;
                case RatingType.Comment:
                    throw new NotImplementedException("Comment rating type has not been implemented");
                default:
                    throw new ArgumentException("Invalid rating type. Make sure you added the type to the switch statement");
            }
        } 

    }
}
