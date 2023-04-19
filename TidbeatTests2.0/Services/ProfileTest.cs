using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tidbeat.Controllers;
using Tidbeat.Data;
using Tidbeat.Models;
using Tidbeat.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace TidbeatTests2._0.Services
{
    public class ProfileTest
    {
        private ApplicationDbContext _context;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        public ProfileTest()
        {
            var fixture = new ApplicationDbContextFixture();
            _context = fixture.ApplicationDbContext;
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
            var normalUser = new ApplicationUser
            {
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
            _context.Users.Add(normalUser);
            _context.SaveChanges();
        }

        [Fact]
        public async Task DetailsProfilesControllerTestAsync()
        {
            var mockEnv = new Mock<IWebHostEnvironment>();
            mockEnv.Setup(m => m.EnvironmentName).Returns("Test");
            // Arrange
            var controller = new ProfilesController(_context, _userManagerMock.Object, mockEnv.Object);
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            controller.TempData = tempData;
            Console.WriteLine(_context.Users.First().Id);
            // Act
            var result = await controller.Details(_context.Users.First().Id.ToString());

            // Assert
            var viewResult = Assert.IsAssignableFrom<IActionResult>(result);

        }

        
    }

}
