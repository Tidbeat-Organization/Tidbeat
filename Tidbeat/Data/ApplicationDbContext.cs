﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tidbeat.Models;

namespace Tidbeat.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Song> Songs { get; set; }
        public DbSet<Band> Bands { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostRating> PostRatings { get; set; }
        public DbSet<CommentRating> CommentRatings { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Participant> Participants { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Song>().ToTable(nameof(Song));
            builder.Entity<Band>().ToTable(nameof(Band));
            builder.Entity<Post>().ToTable(nameof(Post));
            builder.Entity<PostRating>().ToTable(nameof(PostRating));
            builder.Entity<CommentRating>().ToTable(nameof(CommentRating));
        }

        public DbSet<Tidbeat.Models.Profile>? Profile { get; set; }

        public DbSet<Tidbeat.Models.Follow>? Follow { get; set; }
    }
}