﻿using Microsoft.AspNetCore.Identity;
using Tidbeat.Data;
using Tidbeat.Models;

namespace Tidbeat {
    public static class Configurations {
        public static async Task CreateStartingUsers(IServiceProvider serviceProvider) {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var normalUser = new ApplicationUser {
                FullName = "Utilizador Normal",
                UserName = "afonsosemeano@gmail.com",
                Email = "afonsosemeano@gmail.com",
                BirthdayDate = DateTime.Now,
                Gender = "Masculino",
                FavoriteSongId = null
            };

            //var createUser = await userManager.CreateAsync(normalUser, "Password_123");
        }

        public static async Task CreateStartingPosts(IServiceProvider serviceProvider) {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            Band band = new Band() {
                BandId = "2HHmvvSQ44ePDH7IKVzgK0",
                Name = "Jain",
                Image = "https://i.scdn.co/image/ab67616d0000b2732b8f7f8f7f8f7f8f7f8f7f8f"
            };
            var song = new Song() {
                SongId = "0F3fscqwFqD52Tpbep5hBW",
                Name = "Oh Man",
                Band = band
            };

            var post1 = new Post() {
                Title = "Amazing song",
                Content = "This is an amazing song",
                Song = song,
                Band = null,
                User = await userManager.FindByEmailAsync("afonsosemeano@gmail.com")
            };
            var rating1 = new PostRating() {
                Value = 5,
                User = await userManager.FindByEmailAsync("afonsosemeano@gmail.com"),
                Post = post1,
            };
            var post2 = new Post() {
                Title = "Great song",
                Content = "This is an amazing song",
                Song = song,
                Band = null,
                User = await userManager.FindByEmailAsync("afonsosemeano@gmail.com")
            };
            var rating2 = new PostRating() {
                Value = 2,
                User = await userManager.FindByEmailAsync("afonsosemeano@gmail.com"),
                Post = post2,
            };
            var post3 = new Post() {
                Title = "Incredible song",
                Content = "This is an amazing song",
                Song = song,
                Band = null,
                User = await userManager.FindByEmailAsync("afonsosemeano@gmail.com")
            };
            var post4 = new Post() {
                Title = "Unbelievable song",
                Content = "This is an amazing song",
                Song = song,
                Band = null,
                User = await userManager.FindByEmailAsync("afonsosemeano@gmail.com")
            };
            var post5 = new Post() {
                Title = "Squabble song",
                Content = "This is an amazing song",
                Song = song,
                Band = null,
                User = await userManager.FindByEmailAsync("afonsosemeano@gmail.com")
            };

            var post6 = new Post() {
                Title = "I luv this band",
                Content = "This is an amazing band",
                Song = null,
                Band = band,
                User = await userManager.FindByEmailAsync("afonsosemeano@gmail.com")
            };
            var post7 = new Post() {
                Title = "Squabble song",
                Content = "This is an amazing song",
                Song = null,
                Band = band,
                User = await userManager.FindByEmailAsync("afonsosemeano@gmail.com")
            };
            var post8 = new Post() {
                Title = "Squabble song",
                Content = "This is an amazing song",
                Song = null,
                Band = band,
                User = await userManager.FindByEmailAsync("afonsosemeano@gmail.com")
            };
            var post9 = new Post() {
                Title = "Squabble song",
                Content = "This is an amazing song",
                Song = null,
                Band = band,
                User = await userManager.FindByEmailAsync("afonsosemeano@gmail.com")
            };

            context.Songs.Add(song);
            context.Posts.Add(post1);
            context.Posts.Add(post2);
            context.Posts.Add(post3);
            context.Posts.Add(post4);
            context.Posts.Add(post5);
            context.Posts.Add(post6);
            context.Posts.Add(post7);
            context.Posts.Add(post8);
            context.Posts.Add(post9);
            context.PostRatings.Add(rating1);
            context.PostRatings.Add(rating2);
            context.SaveChanges();
        }
    }
}
