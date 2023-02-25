using Microsoft.AspNetCore.Identity;
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
                Gender = "Masculino"
            };

            var createUser = await userManager.CreateAsync(normalUser, "Password_123");
        }

        public static async Task CreateStartingPosts(IServiceProvider serviceProvider) {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var song = new Song() {
                SongId = "0F3fscqwFqD52Tpbep5hBW",
                Name = "Oh Man",
                Band = new Band() {
                    BandId = "2HHmvvSQ44ePDH7IKVzgK0",
                    Name = "Jain",
                    Image = "https://i.scdn.co/image/ab67616d0000b2732b8f7f8f7f8f7f8f7f8f7f8f"
                }
            };

            var post1 = new Post() {
                Title = "Amazing song",
                Content = "This is an amazing song",
                Song = song,
                User = await userManager.FindByEmailAsync("afonsosemeano@gmail.com")
            };
            var post2 = new Post() {
                Title = "Great song",
                Content = "This is an amazing song",
                Song = song,
                User = await userManager.FindByEmailAsync("afonsosemeano@gmail.com")
            };
            var post3 = new Post() {
                Title = "Incredible song",
                Content = "This is an amazing song",
                Song = song,
                User = await userManager.FindByEmailAsync("afonsosemeano@gmail.com")
            };
            var post4 = new Post() {
                Title = "Unbelievable song",
                Content = "This is an amazing song",
                Song = song,
                User = await userManager.FindByEmailAsync("afonsosemeano@gmail.com")
            };
            var post5 = new Post() {
                Title = "Squabble song",
                Content = "This is an amazing song",
                Song = song,
                User = await userManager.FindByEmailAsync("afonsosemeano@gmail.com")
            };

            context.Songs.Add(song);
            context.Posts.Add(post1);
            context.Posts.Add(post2);
            context.Posts.Add(post3);
            context.Posts.Add(post4);
            context.Posts.Add(post5);
            context.SaveChanges();
        }
    }
}
