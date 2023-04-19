using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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

namespace TidbeatTests2._0.Services
{
    public class FollowsControllerTests
    {
        private ApplicationDbContext _context;
        private IServiceProvider _serviceProvider;
        private Mock<UserManager<ApplicationUser>> _userManager;

        public FollowsControllerTests()
        {
            var fixture = new ApplicationDbContextFixture();
            _context = fixture.ApplicationDbContext;
            _serviceProvider = new MockServiceProvider();
            _userManager = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(),
                null, null, null, null, null, null, null, null
            );
        }


        [Fact]
        public async Task Followers_ReturnsJsonResult()
        {
            // Arrange
            var controller = new FollowsController(_context, _serviceProvider);
            var userId = "1"; // Set the user ID for testing

            // Act
            var result = await controller.Followers(userId);

            // Assert
            Assert.IsType<JsonResult>(result);
        }

        [Fact]
        public async Task Followies_ReturnsJsonResult()
        {
            // Arrange
            var controller = new FollowsController(_context, _serviceProvider);
            var userId = "1"; // Set the user ID for testing

            // Act
            var result = await controller.Followies(userId);

            // Assert
            Assert.IsType<JsonResult>(result);
        }

        [Fact]
        public async Task Follow_ReturnsJsonResult()
        {
            // Arrange
            var controller = new FollowsController(_context, _serviceProvider);
            var userId = "1"; // Set the user ID for testing

            // Act
            var result = await controller.Follow(userId);

            // Assert
            Assert.IsType<JsonResult>(result);
        }

        [Fact]
        public async Task UnFollow_ReturnsJsonResult()
        {
            // Arrange
            var controller = new FollowsController(_context, _serviceProvider);
            var userId = "1"; // Set the user ID for testing

            // Act
            var result = await controller.UnFollow(userId);

            // Assert
            Assert.IsType<JsonResult>(result);
        }
    }

}
