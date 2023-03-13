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
            double average = 0;
            switch (type) {
                case RatingType.Post:
                    var allPostRatings = _context.PostRatings.Where(r => r.Post.PostId == id && r.Value > 0);
                    if (allPostRatings.Any()) {
                        average = await allPostRatings.AverageAsync(r => r.Value);
                    }
                    return average;
                case RatingType.Comment:
                    var allCommentRatings = _context.CommentRatings.Where(r => r.Comment.CommentId == id && r.Value > 0);
                    if (allCommentRatings.Any()) {
                        average = await allCommentRatings.AverageAsync(r => r.Value);
                    }
                    return average;
                default:
                    throw new ArgumentException("Invalid rating type. Make sure you added the type to the switch statement");
            }
        }

        public async Task<bool> HasUserRated(RatingType type, int id, string userId) {
            switch (type) {
                case RatingType.Post:
                    return await _context.PostRatings.Where(r => r.Post.PostId == id && r.User.Id == userId).AnyAsync(r => r.Value > 0);
                case RatingType.Comment:
                    return await _context.CommentRatings.Where(r => r.Comment.CommentId == id && r.User.Id == userId).AnyAsync(r => r.Value > 0);
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
                    var commentRating = await _context.CommentRatings.FirstOrDefaultAsync(r => r.Comment.CommentId == id && r.User.Id == userId);
                    if (commentRating == null) return 0;
                    return commentRating.Value;
                default:
                    throw new ArgumentException("Invalid rating type. Make sure you added the type to the switch statement");
            }
        }

        public async Task SetUserRate(RatingType type, int id, string userId, int value) {
            switch (type) {
                case RatingType.Post:
                    var oldPostRating = await _context.PostRatings.FirstOrDefaultAsync(r => r.Post.PostId == id && r.User.Id == userId);
                    if (oldPostRating != null) {
                        oldPostRating.Value = value;
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
                    var oldCommentRating = await _context.CommentRatings.FirstOrDefaultAsync(r => r.Comment.CommentId == id && r.User.Id == userId);
                    if (oldCommentRating != null) {
                        oldCommentRating.Value = value;
                    } else {
                        var commentRating = new CommentRating() {
                            Comment = await _context.Comment.FindAsync(id),
                            User = await _context.Users.FindAsync(userId),
                            Value = value
                        };
                        _context.CommentRatings.Add(commentRating);
                    }
                    await _context.SaveChangesAsync();
                    break;
                default:
                    throw new ArgumentException("Invalid rating type. Make sure you added the type to the switch statement");
            }
        } 

    }
}
