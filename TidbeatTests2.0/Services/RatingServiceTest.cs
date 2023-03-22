using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tidbeat.Data;
using Tidbeat.Models;
using Tidbeat.Services;

namespace TidbeatTests2._0.Services
{
    public class RatingServiceTest
    {
        private ApplicationDbContext _context;

        public RatingServiceTest()
        {
            var fixture = new ApplicationDbContextFixture();
            _context = fixture.ApplicationDbContext;

            var normalUser = new ApplicationUser {
                FullName = "Utilizador Normal",
                UserName = "user@gmail.com",
                Email = "user@gmail.com",
                BirthdayDate = DateTime.Now,
                Gender = "Masculino",
                FavoriteSongIds = null,
                PasswordHash = null,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0,
            };
            var normalUser1 = new ApplicationUser {
                FullName = "Utilizador Muito Anormal",
                UserName = "anormaluser@gmail.com",
                Email = "anormaluser@gmail.com",
                BirthdayDate = DateTime.Now,
                Gender = "Masculino",
                FavoriteSongIds = null,
                PasswordHash = null,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0,
            };

            _context.Users.Add(normalUser);
            _context.Users.Add(normalUser1);
            _context.SaveChanges();

            var simplePost = new Post() {
                Title = "Test post",
                Content = "Im testing this post",
                User = normalUser
            };

            _context.Posts.Add(simplePost);
            _context.SaveChanges();
        }

        [Fact]
        public async void GetAverageRatingRatingServiceTest() {
            var ratingService = new RatingService(_context);
            var user = _context.Users.FirstOrDefault(u => u.Email == "user@gmail.com");
            var user1 = _context.Users.FirstOrDefault(u => u.Email == "anormaluser@gmail.com");

            var band = new Band() {
                BandId = "66CXWjxzNUsdJxJ2JdwvnR",
                Name = "Ariana Grande",
                Image = "https://i.scdn.co/image/ab6761610000e5ebcdce7620dc940db079bf4952"
            };
            _context.Bands.Add(band);
            _context.SaveChanges();

            var simplePost = _context.Posts.FirstOrDefault(u => u.Title == "Test post");

            ratingService.SetUserRate(Tidbeat.Enums.RatingType.Post, simplePost.PostId, user.Id, 4);
            ratingService.SetUserRate(Tidbeat.Enums.RatingType.Post, simplePost.PostId, user1.Id, 1);

            var averageRating = await ratingService.GetAverageRating(Tidbeat.Enums.RatingType.Post, simplePost.PostId);

            Assert.Equal(2.5, averageRating);
        }

        [Fact]
        public async void HasUserRatedRatingServiceTest() {
            var user = _context.Users.FirstOrDefault(u => u.Email == "user@gmail.com");
            var simplePost = _context.Posts.FirstOrDefault(u => u.Title == "Test post");

            var ratingService = new RatingService(_context);

            await ratingService.SetUserRate(Tidbeat.Enums.RatingType.Post, simplePost.PostId, user.Id, 4);
            var hasRated = await ratingService.HasUserRated(Tidbeat.Enums.RatingType.Post, simplePost.PostId, user.Id);
            Assert.True(hasRated);

            await ratingService.SetUserRate(Tidbeat.Enums.RatingType.Post, simplePost.PostId, user.Id, 0);
            hasRated = await ratingService.HasUserRated(Tidbeat.Enums.RatingType.Post, simplePost.PostId, user.Id);
            Assert.False(hasRated);
        }

        [Theory]
        [InlineData(5)]
        [InlineData(4)]
        [InlineData(3)]
        [InlineData(2)]
        [InlineData(1)]
        public async void GetUserRateRatingServiceTest(int value) {
            var user = _context.Users.FirstOrDefault(u => u.Email == "user@gmail.com");
            var simplePost = _context.Posts.FirstOrDefault(u => u.Title == "Test post");
            

            var ratingService = new RatingService(_context);
            await ratingService.SetUserRate(Tidbeat.Enums.RatingType.Post, simplePost.PostId, user.Id, value);

            var rating = await ratingService.GetUserRate(Tidbeat.Enums.RatingType.Post, simplePost.PostId, user.Id);

            Assert.Equal(value, rating);
        }

        [Fact]
        public async void SetUserRateRatingServiceTest_CreateNewRating()
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == "user@gmail.com");

            var simplePost = _context.Posts.FirstOrDefault(u => u.Title == "Test post");

            var ratingService = new RatingService(_context);

            await ratingService.SetUserRate(Tidbeat.Enums.RatingType.Post, simplePost.PostId, user.Id, 5);

            var rating = _context.PostRatings.Where(r => r.User.Id == user.Id && r.Post.PostId == simplePost.PostId).FirstOrDefault();

            Assert.NotNull(rating);
            Assert.Equal(5, rating.Value);
            Assert.Equal(user.Id, rating.User.Id);
            Assert.Equal(simplePost.PostId, rating.Post.PostId);
        }

        [Fact]
        public async void SetUserRateRatingServiceTest_ReplaceRating()
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == "user@gmail.com");

            var simplePost = _context.Posts.FirstOrDefault(u => u.Title == "Test post");

            var ratingService = new RatingService(_context);

            await ratingService.SetUserRate(Tidbeat.Enums.RatingType.Post, simplePost.PostId, user.Id, 5);

            var rating = _context.PostRatings.Where(r => r.User.Id == user.Id && r.Post.PostId == simplePost.PostId).FirstOrDefault();

            await ratingService.SetUserRate(Tidbeat.Enums.RatingType.Post, simplePost.PostId, user.Id, 3);

            Assert.NotNull(rating);
            Assert.Equal(3, rating.Value);
            Assert.Equal(user.Id, rating.User.Id);
            Assert.Equal(simplePost.PostId, rating.Post.PostId);
        }
    }
}
