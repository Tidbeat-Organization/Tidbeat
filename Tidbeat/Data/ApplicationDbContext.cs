using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tidbeat.Models;

namespace Tidbeat.Data {
    /// <summary>
    /// The database context for the application.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> {
        /// <summary>
        /// The constructor for the database context.
        /// </summary>
        /// <param name="options">The options for the database context.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// The songs in the database.
        /// </summary>
        public DbSet<Song> Songs { get; set; }

        /// <summary>
        /// The bands in the database.
        /// </summary>
        public DbSet<Band> Bands { get; set; }

        /// <summary>
        /// The posts in the database.
        /// </summary>
        public DbSet<Post> Posts { get; set; }

        /// <summary>
        /// The post ratings in the database.
        /// </summary>
        public DbSet<PostRating> PostRatings { get; set; }

        /// <summary>
        /// The comment ratings in the database.
        /// </summary>
        public DbSet<CommentRating> CommentRatings { get; set; }

        /// <summary>
        /// The comments in the database.
        /// </summary>
        public DbSet<Comment> Comment { get; set; }

        /// <summary>
        /// The messages in the database.
        /// </summary>
        public DbSet<Message> Messages { get; set; }

        /// <summary>
        /// The conversations in the database.
        /// </summary>
        public DbSet<Conversation> Conversations { get; set; }

        /// <summary>
        /// The participants in the database.
        /// </summary>
        public DbSet<Participant> Participants { get; set; }

        /// <summary>
        /// The user profiles in the database.
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);
            builder.Entity<Song>().ToTable(nameof(Song));
            builder.Entity<Band>().ToTable(nameof(Band));
            builder.Entity<Post>().ToTable(nameof(Post));
            builder.Entity<PostRating>().ToTable(nameof(PostRating));
            builder.Entity<CommentRating>().ToTable(nameof(CommentRating));

            builder.Entity<Report>()
            .HasOne(r => r.UserReporter)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);
        }

        /// <summary>
        /// The profiles in the database.
        /// </summary>
        public DbSet<Tidbeat.Models.Profile>? Profile { get; set; }

        /// <summary>
        /// The follows in the database.
        /// </summary>
        public DbSet<Tidbeat.Models.Follow>? Follow { get; set; }

        /// <summary>
        /// The reports in the database.
        /// </summary>
        public DbSet<Tidbeat.Models.Report>? Report { get; set; }
    }
}