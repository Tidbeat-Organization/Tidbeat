using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SpotifyAPI.Web;
using Tidbeat.Areas.Identity.Pages.Account;
using Tidbeat.Data;
using Tidbeat.Models;

namespace Tidbeat {
    /// <summary>
    /// This is a class that contains all the starting configurations for the application.
    /// </summary>
    public static class Configurations {
        /// <summary>
        /// This is the starting user that will be created when the application starts. It is used for deleted users. 
        /// You should only use it for fetching the user in the database.
        /// </summary>
        public static ApplicationUser InvalidUser = new ApplicationUser {
            Id = new Guid("00000000-0000-0000-0000-000000000000").ToString(),
            FullName = "[deleted]",
            UserName = "invalid@email.com",
            Email = "invalid@email.com",
            BirthdayDate = DateTime.Now,
            Gender = "male",
            IsBanned = false,
            Role = Enums.RoleType.NormalUser
        };

        public static ApplicationUser AdminUser = new ApplicationUser
        {
            Id = new Guid("00000000-0000-0000-0000-000000000001").ToString(),
            FullName = "admin",
            UserName = "admin@email.com",
            Email = "admin@email.com",
            BirthdayDate = DateTime.Now,
            Gender = "male",
            IsBanned = false,
            Role = Enums.RoleType.Admin,
            EmailConfirmed = true,
        };

        public static ApplicationUser ModUser = new ApplicationUser
        {
            Id = new Guid("00000000-0000-0000-0000-000000000002").ToString(),
            FullName = "mod",
            UserName = "mod@email.com",
            Email = "mod@email.com",
            BirthdayDate = DateTime.Now,
            Gender = "male",
            IsBanned = false,
            Role = Enums.RoleType.Moderator,
            EmailConfirmed = true,
        };

        /// <summary>
        /// This will add an "invalid" user to the database. This user is used for deleted users.
        /// </summary>
        /// <param name="serviceProvider">The service provider used for getting a user manager.</param>
        /// <returns></returns>
        public static async Task CreateStartingUsers(IServiceProvider serviceProvider) {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var token = "";
            var deletedUserExists = await userManager.FindByEmailAsync(InvalidUser.Email);
            if (deletedUserExists == null) {
                await userManager.CreateAsync(InvalidUser);
            }
            userManager.PasswordValidators.Clear();
            userManager.PasswordValidators.Add(new CustomPasswordValidator<ApplicationUser>());
            var modUserExists = await userManager.FindByEmailAsync(ModUser.Email);
            if (modUserExists == null)
            {
               var resultmod = await userManager.CreateAsync(ModUser, "ModPassword1");
                if (resultmod.Succeeded)
                {
                    await userManager.AddToRoleAsync(ModUser, "Moderator");
                }
            }
            var adminUserExists = await userManager.FindByEmailAsync(AdminUser.Email);
            if (adminUserExists == null)
            {
                var resultadmin = await userManager.CreateAsync(AdminUser,"AdminPassword1");
                if (resultadmin.Succeeded)
                {
                    await userManager.AddToRoleAsync(AdminUser, "Admin");
                }
            }
            //var createUser = await userManager.CreateAsync(normalUser, "Password_123");
        }

        /// <summary>
        /// This will create the starting roles for the application. The roles are: Admin, Moderator, NormalUser.
        /// </summary>
        /// <param name="serviceProvider">The service provider used for getting a role manager.</param>
        /// <returns></returns>
        public static async Task CreateStartingRoles(IServiceProvider serviceProvider) {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            string[] rolesNames = { "Admin", "Moderator", "NormalUser" };
            IdentityResult result;
            foreach (var namesRole in rolesNames) {
                var roleExist = await roleManager.RoleExistsAsync(namesRole);
                if (!roleExist) {
                    result = await roleManager.CreateAsync(new IdentityRole(namesRole));
                }
            }
        }

        /// <summary>
        /// This will create the starting posts for the application. Only use this method for testing purposes.
        /// </summary>
        /// <param name="serviceProvider">The service provider used for getting a user manager and a database context.</param>
        /// <returns></returns>
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
            var follow = new Follow() { UserAsker = await userManager.FindByEmailAsync(InvalidUser.Email), UserFollowed= await userManager.FindByEmailAsync(InvalidUser.Email)};
            context.Follow.Add(follow);
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
