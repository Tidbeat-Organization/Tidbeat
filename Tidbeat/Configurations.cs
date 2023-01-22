using Microsoft.AspNetCore.Identity;
using Tidbeat.Data;
using Tidbeat.Models;

namespace Tidbeat {
    public static class Configurations {
        public static async Task CreateStartingUsers(IServiceProvider serviceProvider) {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var normalUser = new ApplicationUser {
                FullName = "Utilizador Normal",
                UserName = "utilizador@ips.pt",
                Email = "utilizador@ips.pt"
            };

            var createUser = await userManager.CreateAsync(normalUser, "Password_123");
        }
    }
}
