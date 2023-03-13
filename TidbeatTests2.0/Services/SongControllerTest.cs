using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
            _spotify = new SpotifyService();
        }

        [Fact]
        public async Task IndexBandsControllerTestAsync()
        {
            // Arrange
            var controller = new SongsController(_context, _spotify, _userManager);

            // Act
            var result = await controller.Index("","","","","","");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);

        }

        [Fact]
        public async void DetailsBandsControllerTest()
        {
            // Arrange
            var controller = new SongsController(_context,);

            // Act
            var result = await controller.Details("6ocbgoVGwYJhOv1GgI9NsF");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);

        }
    }
}
