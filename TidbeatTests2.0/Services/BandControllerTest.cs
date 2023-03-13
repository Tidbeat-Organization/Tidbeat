﻿using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tidbeat.Controllers;
using Tidbeat.Data;
using Tidbeat.Services;

namespace TidbeatTests2._0.Services
{
    public class BandControllerTest
    {
        private ApplicationDbContext _context;
        private ISpotifyService _spotify;

        public BandControllerTest()
        {
            var fixture = new ApplicationDbContextFixture();
            _context = fixture.ApplicationDbContext;
            _spotify = new SpotifyService();
        }

        [Fact]
        public async Task IndexBandsControllerTestAsync()
        {
            // Arrange
            var controller = new BandsController(_context,);

            // Act
            var result = await controller.Index("","","");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);

        }

        [Fact]
        public async void DetailsBandsControllerTest()
        {
            // Arrange
            var controller = new BandsController(_context,);

            // Act
            var result = await controller.Details("66CXWjxzNUsdJxJ2JdwvnR");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);

        }
    }
}
