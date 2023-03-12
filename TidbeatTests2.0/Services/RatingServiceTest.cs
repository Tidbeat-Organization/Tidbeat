using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
        private UserManager<ApplicationUser> _userManager;

        public RatingServiceTest()
        {
            var fixture = new ApplicationDbContextFixture();
            _context = fixture.ApplicationDbContext;
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_context), null, null, null, null, null, null, null, null);
        }
        /*
        public async void GetAverageRatingRatingServiceTest() {
            var ratingService = new RatingService(_context);
            // Arrange

            // Act


            // Assert
            // Assert something about the result
            var normalUser = new ApplicationUser {
                FullName = "Utilizador Normal",
                UserName = "user@gmail.com",
                Email = "user@gmail.com",
                BirthdayDate = DateTime.Now,
                Gender = "Masculino",
                FavoriteSongId = null
            };

            await _userManager.CreateAsync(normalUser, "Password_123");

            var band = new Band() {
                BandId = "66CXWjxzNUsdJxJ2JdwvnR",
                Name = "Ariana Grande",
                Image = "https://i.scdn.co/image/ab6761610000e5ebcdce7620dc940db079bf4952"
            };
            _context.Bands.Add(band);
            _context.SaveChanges();

            var rating = new PostRating() {
                Value = 5,
                User = await _userManager.FindByEmailAsync("user@gmail.com"),
                Post = new Post() {
                    Title = "Test post",
                    Content = "Im testing this post"
                }

            };
            //_context.Ratings.Add(rating);
            _context.SaveChanges();

            //var averageRating = ratingService.GetAverageRating("66CXWjxzNUsdJxJ2JdwvnR");

            //Assert.Equal(5, averageRating);
        }*/

        public void HasUserRatedRatingServiceTest() {

        }

        public void GetUserRateRatingServiceTest() {

        }

        [Fact]
        public async void SetUserRateRatingServiceTest_CreateNewRating()
        {
            var normalUser = new ApplicationUser
            {
                FullName = "Utilizador Normal",
                UserName = "user@gmail.com",
                Email = "user@gmail.com",
                BirthdayDate = DateTime.Now,
                Gender = "Masculino",
                FavoriteSongId = null,
                PasswordHash = null,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0,
            };

            await _context.Users.AddAsync(normalUser);
            await _context.SaveChangesAsync();
            var user = _context.Users.FirstOrDefault(u => u.Email == "user@gmail.com");

            var simplePost = new Post()
            {
                Title = "Test post",
                Content = "Im testing this post",
                User = user
            };

            _context.Posts.Add(simplePost);
            _context.SaveChanges();

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
            var normalUser = new ApplicationUser
            {
                FullName = "Utilizador Normal",
                UserName = "user@gmail.com",
                Email = "user@gmail.com",
                BirthdayDate = DateTime.Now,
                Gender = "Masculino",
                FavoriteSongId = null,
                PasswordHash = null,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0,
            };
            await _context.Users.AddAsync(normalUser);
            await _context.SaveChangesAsync();
            var user = _context.Users.FirstOrDefault(u => u.Email == "user@gmail.com");

            var simplePost = new Post()
            {
                Title = "Test post",
                Content = "Im testing this post",
                User = user
            };

            _context.Posts.Add(simplePost);
            _context.SaveChanges();

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
