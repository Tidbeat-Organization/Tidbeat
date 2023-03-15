using Microsoft.Extensions.Configuration;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tidbeat.Services;

namespace TidbeatTests2._0.Services
{
    public class SpotifyServiceTestPartII
    {

        private readonly ISpotifyService _spotifyService;


        public SpotifyServiceTestPartII()
        {
            var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
            _spotifyService = new SpotifyService(configuration);
        }

        [Fact]
        public async Task GetMultipleBandsAsyncSpotifyServiceTest()
        {
            // Arrange
            string searchKey = "Man";

            // Act
            var result = await _spotifyService.GetMultipleBandsAsync(searchKey);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<SearchResponse>(result);
            Assert.NotEmpty(result.Artists.Items);
        }

        [Fact]
        public async Task GetSearchBandsbyValuesAsyncSpotifyServiceTest()
        {
            // Arrange
            string searchKey = "Man";
            string genre = "rock";

            // Act
            var result = await _spotifyService.GetSearchBandsbyValuesAsync(searchKey, genre);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<SearchResponse>(result);
            Assert.NotEmpty(result.Artists.Items);
        }

    }
}