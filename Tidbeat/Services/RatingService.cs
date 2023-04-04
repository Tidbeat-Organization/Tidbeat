using Microsoft.EntityFrameworkCore;
using Tidbeat.Data;
using Tidbeat.Enums;
using Tidbeat.Models;

namespace Tidbeat.Services {
    /// <summary>
    /// It takes care of all operations related to ratings.
    /// </summary>
    public class RatingService : IRatingService {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Creates a new instance of the RatingService.
        /// </summary>
        /// <param name="context">The context of the application.</param>
        public RatingService(ApplicationDbContext context) {
            _context = context;
        }

        /// <summary>
        /// Gets the average rating of a post or comment.
        /// </summary>
        /// <param name="type">The type of the object to get the rating from.</param>
        /// <param name="id">The id of the object to get the rating from.</param>
        /// <returns>The average rating of the object.</returns>
        /// <exception cref="ArgumentException"></exception>
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

        /// <summary>
        /// Gets whether the user has rated or not.
        /// </summary>
        /// <param name="type">The type of the object to get the rating from.</param>
        /// <param name="id">The id of the object to get the rating from.</param>
        /// <param name="userId">The id of the user to check if he rated or not.</param>
        /// <returns>True if the user has rated, false otherwise.</returns>
        /// <exception cref="ArgumentException"></exception>
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

        /// <summary>
        /// Gets the user's rating. If the user hasn't rated, it returns 0.
        /// </summary>
        /// <param name="type">The type of the object to get the rating from.</param>
        /// <param name="id">The id of the object to get the rating from.</param>
        /// <param name="userId">The id of the user to get the rating from.</param>
        /// <returns>The user's rating.</returns>
        /// <exception cref="ArgumentException"></exception>
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

        /// <summary>
        /// Sets the user's rating.
        /// </summary>
        /// <param name="type">The type of the object to get the rating from.</param>
        /// <param name="id">The id of the object to get the rating from.</param>
        /// <param name="userId">The id of the user to set the rating from.</param>
        /// <param name="value">The value of the rating.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
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
