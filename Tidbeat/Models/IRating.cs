namespace Tidbeat.Models {
    /// <summary>
    /// An interface in case you need to add a rating system to any kind of entity. If you need to do that, just create a new model and implement this interface.
    /// Then, make sure to turn that model into a DbSet in the ApplicationDbContext class, add a new entry in the RatingType enum then add a new condition to all switches in the RatingService class.
    /// That way, you can get the rating of any entity.
    /// </summary>
    public interface IRating {
        /// <summary>
        /// The rating id.
        /// </summary>
        public int RatingId { get; set; }

        /// <summary>
        /// The rating's value.
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// The rating's owner.
        /// </summary>
        public ApplicationUser User { get; set; }
    }
}
