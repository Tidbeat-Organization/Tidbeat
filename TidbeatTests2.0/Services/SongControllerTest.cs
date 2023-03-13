using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tidbeat.Controllers;
using Tidbeat.Data;
using Tidbeat.Models;
using Tidbeat.Services;

namespace TidbeatTests2._0.Services
{
    public class SongControllerTest
    {
        private ApplicationDbContext _context;
        private ISpotifyService _spotify;
        private UserManager<ApplicationUser> _userManager;

        public SongControllerTest()
        {
            var fixture = new ApplicationDbContextFixture();
            _context = fixture.ApplicationDbContext;
            _userManager = fixture.UserManager;
            _spotify = new MockSpotifyService();
        }

        [Fact]
        public async Task IndexBandsControllerTestAsync()
        {
            // Arrange
            var controller = new SongsController(_context, _spotify, _userManager);
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            controller.TempData = tempData;


            // Act
            var result = await controller.Index("","","","","","");

            // Assert
            var viewResult = Assert.IsAssignableFrom<IActionResult>(result);

        }

        [Fact]
        public async void DetailsBandsControllerTest()
        {
            // Arrange
            var controller = new SongsController(_context, _spotify, _userManager);
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            controller.TempData = tempData;


            // Act
            var result = await controller.Details("6ocbgoVGwYJhOv1GgI9NsF");

            // Assert
            var viewResult = Assert.IsAssignableFrom<IActionResult>(result);

        }
    }
}
