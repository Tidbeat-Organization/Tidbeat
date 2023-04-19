using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tidbeat.Controllers;
using Tidbeat.Data;
using Tidbeat.Services;
using Tidbeat.Models;
using Microsoft.AspNetCore.Identity;

namespace TidbeatTests2._0.Services
{
    public class BandControllerTest
    {
        private ApplicationDbContext _context;
        private ISpotifyService _spotify;
        private Mock<UserManager<ApplicationUser>> _userManager;

        public BandControllerTest()
        {
            var fixture = new ApplicationDbContextFixture();
            _context = fixture.ApplicationDbContext;
            _spotify = new MockSpotifyService();
            _userManager = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(),
                null, null, null, null, null, null, null, null
            );
        }

        [Fact]
        public async Task IndexBandsControllerTestAsync()
        {
            // Arrange
            var controller = new BandsController(_context, _spotify, _userManager.Object);
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            controller.TempData = tempData;

            // Act
            var result = await controller.Index("name","name","name");

            // Assert
            var viewResult = Assert.IsAssignableFrom<IActionResult>(result);

        }
        
        [Fact]
        public async void DetailsBandsControllerTest()
        {
            // Arrange
            var controller = new BandsController(_context, _spotify, _userManager.Object);

            // Act
            var result = await controller.Details("66CXWjxzNUsdJxJ2JdwvnR");

            // Assert
            var viewResult = Assert.IsAssignableFrom<IActionResult>(result);

        }
    }
}
