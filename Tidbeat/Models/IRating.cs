namespace Tidbeat.Models {
    public interface IRating {
        public int RatingId { get; set; }
        public int Value { get; set; }

        public ApplicationUser User { get; set; }
    }
}
