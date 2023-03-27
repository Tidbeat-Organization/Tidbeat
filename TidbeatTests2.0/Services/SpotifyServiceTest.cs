using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tidbeat.Data;
using Tidbeat.Models;
using Tidbeat.Services;

namespace TidbeatTests2._0.Services
{
    public class SpotifyServiceTest
    {
        private ApplicationDbContext _context;
        public SpotifyServiceTest()
        {
            var fixture = new ApplicationDbContextFixture();
            _context = fixture.ApplicationDbContext;
        }

        [Fact]
        public async void GetSongAsyncSpotifyServiceTest()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                    {
                    {"SpotifyAPI:ClientId", "b7e13ced889d41389c8f93767c3803d3"},
                    {"SpotifyAPI:ClientSecret", "d82a544b10394f07b3ce80c10729f0a4"}
                     })
                .Build();
            //IConfiguration config = Mock.Of<IConfiguration>();
            //config["SpotifyAPI:ClientId"] = "b7e13ced889d41389c8f93767c3803d3";
            //config["SpotifyAPI:ClientSecret"] = "d82a544b10394f07b3ce80c10729f0a4";
            var apiSpotify = new SpotifyService(config);
            var song = await apiSpotify.GetSongAsync("2t7FwwZasrW1hZGdIAVCAe");
            Assert.False(song == null);
        }

        [Fact]
        public async void GetBandAsyncSpotifyServiceTest()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                    {
                    {"SpotifyAPI:ClientId", "b7e13ced889d41389c8f93767c3803d3"},
                    {"SpotifyAPI:ClientSecret", "d82a544b10394f07b3ce80c10729f0a4"}
                     })
                .Build();
            //IConfiguration config = Mock.Of<IConfiguration>();
            //config["SpotifyAPI:ClientId"] = "b7e13ced889d41389c8f93767c3803d3";
            //config["SpotifyAPI:ClientSecret"] = "d82a544b10394f07b3ce80c10729f0a4";
            var apiSpotify = new SpotifyService(config);
            var band = await apiSpotify.GetBandAsync("2bdcBjvuI9worc472GbeU0");
            
            Assert.False(band == null);
        }

        [Fact]
        public async void GetAmountBandAlbumAsyncSpotifyServiceTest()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                    {
                    {"SpotifyAPI:ClientId", "b7e13ced889d41389c8f93767c3803d3"},
                    {"SpotifyAPI:ClientSecret", "d82a544b10394f07b3ce80c10729f0a4"}
                     })
                .Build();
            //IConfiguration config = Mock.Of<IConfiguration>();
            //config["SpotifyAPI:ClientId"] = "b7e13ced889d41389c8f93767c3803d3";
            //config["SpotifyAPI:ClientSecret"] = "d82a544b10394f07b3ce80c10729f0a4";
            var apiSpotify = new SpotifyService(config);
            var albums = await apiSpotify.GetAmountBandAlbumAsync("0u18Cq5stIQLUoIaULzDmA");
            Assert.True(albums == 2);
        }

        [Fact]
        public async void GetTop3SongsAsyncSpotifyServiceTest()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                    {
                    {"SpotifyAPI:ClientId", "b7e13ced889d41389c8f93767c3803d3"},
                    {"SpotifyAPI:ClientSecret", "d82a544b10394f07b3ce80c10729f0a4"}
                     })
                .Build();
            //IConfiguration config = Mock.Of<IConfiguration>();
            //config["SpotifyAPI:ClientId"] = "b7e13ced889d41389c8f93767c3803d3";
            //config["SpotifyAPI:ClientSecret"] = "d82a544b10394f07b3ce80c10729f0a4";
            var apiSpotify = new SpotifyService(config);
            var top3Songs = await apiSpotify.GetTop3SongsAsync("0hmhv5CTaXOilOf0hGeIvN");
            string top3SongString = top3Songs.Select(s => s.Name).Aggregate((a, b) => a + ";" + b);
            Console.WriteLine(top3SongString);

            Assert.Equal("Ave verum;Lully, Lulla Lullay;Do Not Be Afraid", top3SongString);
        }

        [Fact]
        public async void GetMultipleSongsAsyncSpotifyServiceTest()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                    {
                    {"SpotifyAPI:ClientId", "b7e13ced889d41389c8f93767c3803d3"},
                    {"SpotifyAPI:ClientSecret", "d82a544b10394f07b3ce80c10729f0a4"}
                     })
                .Build();
            //IConfiguration config = Mock.Of<IConfiguration>();
            //config["SpotifyAPI:ClientId"] = "b7e13ced889d41389c8f93767c3803d3";
            //config["SpotifyAPI:ClientSecret"] = "d82a544b10394f07b3ce80c10729f0a4";
            var apiSpotify = new SpotifyService(config);
            // 0SVE49uB6pYy9g9h7qg0Pq -> "Ave verum"
            // 4g7HVZft9KyKFoo3d0WxPc -> "Lully, Lulla Lullay"
            var search = await apiSpotify.GetMultipleSongsAsync("fur elise");
            var songs = search.Tracks.Items;
            var songNames = songs.Select(s => s.Name);
            string songsString = songNames.Aggregate((a, b) => a + ";" + b);
            
            // Line solely for the purpose of preparing the test.
            File.WriteAllText("songs.txt", songNames.Aggregate((a, b) => a + ";" + b));

            Assert.True(
                songNames.Contains("Für Elise - Reimagined") &&
                songNames.Contains("Für Elise, WoO 59")
            );

        }

        [Fact]
        public async void GetSearchSongsbyValuesAsyncSpotifyServiceTest()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                    {
                    {"SpotifyAPI:ClientId", "b7e13ced889d41389c8f93767c3803d3"},
                    {"SpotifyAPI:ClientSecret", "d82a544b10394f07b3ce80c10729f0a4"}
                     })
                .Build();
            //IConfiguration config = Mock.Of<IConfiguration>();
            //config["SpotifyAPI:ClientId"] = "b7e13ced889d41389c8f93767c3803d3";
            //config["SpotifyAPI:ClientSecret"] = "d82a544b10394f07b3ce80c10729f0a4";
            var apiSpotify = new SpotifyService(config);
            var search = await apiSpotify.GetSearchSongsbyValuesAsync("piano", "classical", "", "", "", "");
            var songs = search.Tracks.Items;
            var songNames = songs.Select(s => s.Name);

            // Line solely for the purpose of preparing the test.
            File.WriteAllText("songs_b.txt", songNames.Aggregate((a, b) => a + ";" + b));

            Assert.True(songNames.Contains("The Carnival of the Animals, R. 125: XIII. The Swan (Arr. for Cello and Piano)"));
        }
    }
}
