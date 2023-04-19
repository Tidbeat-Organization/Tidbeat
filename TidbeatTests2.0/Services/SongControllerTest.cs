using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Controller;
using Moq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
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
        private Mock<UserManager<ApplicationUser>> _userManager;

        public SongControllerTest()
        {
            var fixture = new ApplicationDbContextFixture();
            _context = fixture.ApplicationDbContext;
            _userManager = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(),
                null, null, null, null, null, null, null, null
            );
            _spotify = new MockSpotifyService();
        }

        [Fact]
        public async Task IndexSongsControllerTestAsync()
        {
            // Arrange
            var controller = new SongsController(_context, _spotify, _userManager.Object);
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            controller.TempData = tempData;


            // Act
            var result = await controller.Index("name","name","name","name","name","name");

            // Assert
            var viewResult = Assert.IsAssignableFrom<IActionResult>(result);

        }

        [Fact]
        public async Task DetailsSongsControllerTest()
        {
            // Arrange
            var controller = new BandsController(_context, _spotify, _userManager.Object);

            // Act
            var result = await controller.Details("6ocbgoVGwYJhOv1GgI9NsF");

            // Assert
            var viewResult = Assert.IsAssignableFrom<IActionResult>(result);
        }

    }
}
