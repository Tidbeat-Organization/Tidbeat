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

            var user = new ApplicationUser
            {
                Id = "1",
                FullName = "José Maria",
                UserName = "josemaria@gmail.com",
                Email = "josemaria@gmail.com",
                BirthdayDate = DateTime.Now,
                Gender = "Masculino",
                FavoriteSongIds = null,
                PasswordHash = null,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0,
                Role = RoleType.NormalUser
            };

            var mod = new ApplicationUser
            {
                Id = "2",
                FullName = "Mariana Vilares",
                UserName = "marianavilares@gmail.com",
                Email = "josemaria@gmail.com",
                BirthdayDate = DateTime.Now,
                Gender = "Feminino",
                FavoriteSongIds = null,
                PasswordHash = null,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0,
                Role = RoleType.Moderator
            };

            var admin = new ApplicationUser
            {
                Id = "3",
                FullName = "Sam Faial",
                UserName = "samfaial@gmail.com",
                Email = "samfaial@gmail.com",
                BirthdayDate = DateTime.Now,
                Gender = "Não-Binário",
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
            _context.SaveChanges();
        }


        [Fact]
        public async Task ConcedePriviledgeTestAsync()
        {
            // Test conceding priviledges to a user.
            var roleController = new RoleController(_context, _userManagerMock.Object, null, null, null);

            // Arrange
            var oldRoleNormalUser = _context.Users.First(u => u.Id == "1").Role;
            

            await roleController.GivePermisson("1", RoleType.Moderator);

            var newRoleNormalUser = _context.Users.First(u => u.Id == "1").Role;

            Assert.True(oldRoleNormalUser == RoleType.NormalUser && newRoleNormalUser == RoleType.Moderator);

        }
        
        [Fact]
        public async Task RemoveUserTestAsync()
        {
            var roleController = new RoleController(_context, _userManagerMock.Object, null, null, null);

            await roleController.DeleteAsync(new DeleteAsyncDto() { UserId="1" });

            Assert.False(_context.Users.Any(u => u.Id == "1"));
        }

        
    }
}
