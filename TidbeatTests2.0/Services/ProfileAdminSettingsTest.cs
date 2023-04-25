using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tidbeat.Controllers;
using Tidbeat.Data;
using Tidbeat.Models;
using Tidbeat.Enums;
using Tidbeat.DTOs.Role;
using Tidbeat;
using Microsoft.Extensions.Localization;

namespace TidbeatTests2._0.Services
{
    public class ProfileAdminSettingsTest
    {
        private ApplicationDbContext _context;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;

        public ProfileAdminSettingsTest()
        {
            var fixture = new ApplicationDbContextFixture();
            _context = fixture.ApplicationDbContext;

            _userManagerMock = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            
        }


        [Fact]
        public async Task ConcedePriviledgeTestAsync()
        {
            addUsersToFixture();
            // Test conceding priviledges to a user.
            var roleController = new RoleController(_context, _userManagerMock.Object, null, null, null);
            _userManagerMock.Setup(u => u.RemoveFromRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
               .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
               .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<string>()))
               .ReturnsAsync(_context.Users.FirstOrDefault(u => u.Id == "1"));
            // Arrange
            var oldRoleNormalUser = _context.Users.First(u => u.Id == "1").Role;


            await roleController.GivePermisson("1", RoleType.Moderator);

            var newRoleNormalUser = _context.Users.First(u => u.Id == "1").Role;

            Assert.True(oldRoleNormalUser == RoleType.NormalUser && newRoleNormalUser == RoleType.Moderator);

        }

        [Fact]
        public async Task RemoveUserTestAsync()
        {
            addUsersToFixture();
            var user = _context.Users.FirstOrDefault(u => u.Id == "1");
            _userManagerMock.Setup(u => u.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success)
                .Callback<ApplicationUser>(userToDelete => {
                    // Perform the actual delete action here, e.g. using your database context
                    _context.Users.Remove(userToDelete);
                    _context.SaveChanges();
                });
            

            _userManagerMock.Setup(u => u.FindByEmailAsync(Configurations.InvalidUser.Email)).ReturnsAsync(_context.Users.FirstOrDefault(u => u.Id == Configurations.InvalidUser.Id));

            _userManagerMock.Setup(u => u.FindByIdAsync("1")).ReturnsAsync(user);

            _userManagerMock.Setup(u => u.GetUserIdAsync(user)).ReturnsAsync("1");
            var localizerMock = new Mock<IStringLocalizer<RoleController>>();
            localizerMock.Setup(x => x["operation_fail"]).Returns(new LocalizedString("operation_fail", "failed_operation"));
            var roleController = new RoleController(_context, _userManagerMock.Object, null, localizerMock.Object, null);

            

            await roleController.DeleteAsync(new DeleteAsyncDto() { UserId = "1" });

            Assert.False(_context.Users.Any(u => u.Id == "1"));
        }

        [Fact]
        public async Task EditProfileTestAsync()
        {
            addUsersToFixture();

            var user = _context.Users.FirstOrDefault(u => u.Id == "1");
            _userManagerMock.Setup(u => u.FindByIdAsync("1")).ReturnsAsync(user);
            _userManagerMock.Setup(u => u.UpdateAsync(It.IsAny<ApplicationUser>()))
                .Callback<ApplicationUser>(userToUpdate => {
                    _context.Users.Update(userToUpdate);
                }).ReturnsAsync(IdentityResult.Success);

            var localizerMock = new Mock<IStringLocalizer<RoleController>>();
            localizerMock.Setup(x => x["operation_fail"]).Returns(new LocalizedString("operation_fail", "failed_operation"));
            var roleController = new RoleController(_context, _userManagerMock.Object, null, localizerMock.Object, null);

            await roleController.EditAsync(
                new EditAsyncDto()
                {
                    UserId = "1",
                    About = "Sou um utilizador normal."
                }
            );

            Assert.Equal("Sou um utilizador normal.", _context.Users.First(u => u.Id == "1").AboutMe);

        }

        private void addUsersToFixture() {
            var user = new ApplicationUser {
                Id = "1",
                FullName = "José Maria",
                UserName = "josemaria@gmail.com",
                Email = "josemaria@gmail.com",
                BirthdayDate = DateTime.Now,
                Gender = "Masculino",
                AboutMe = "Sou um utilizador normal, pá!!",
                FavoriteSongIds = null,
                PasswordHash = null,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0,
                Role = RoleType.NormalUser
            };

            var mod = new ApplicationUser {
                Id = "2",
                FullName = "Mariana Vilares",
                UserName = "marianavilares@gmail.com",
                Email = "josemaria@gmail.com",
                BirthdayDate = DateTime.Now,
                Gender = "Feminino",
                AboutMe = "Sou uma moderadora",
                FavoriteSongIds = null,
                PasswordHash = null,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0,
                Role = RoleType.Moderator
            };

            var admin = new ApplicationUser {
                Id = "3",
                FullName = "Sam Faial",
                UserName = "samfaial@gmail.com",
                Email = "samfaial@gmail.com",
                BirthdayDate = DateTime.Now,
                Gender = "Não-Binário",
                AboutMe = "Sou da administração",
                FavoriteSongIds = null,
                PasswordHash = null,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0,
                Role = RoleType.Admin
            };

            _context.Users.Add(user);
            _context.Users.Add(mod);
            _context.Users.Add(admin);
            _context.Users.Add(Configurations.InvalidUser);
            _context.SaveChanges();
        }
    }
}
