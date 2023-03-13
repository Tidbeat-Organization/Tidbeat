using Castle.Core.Configuration;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Moq;
using SpotifyAPI.Web;
using Tidbeat.Data;
using Tidbeat.Services;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace TidbeatTests2._0.Services
{
    public class MusicServiceTest
    {
        private ApplicationDbContext _context;

        public MusicServiceTest()
        {
            var fixture = new ApplicationDbContextFixture();
            _context = fixture.ApplicationDbContext;
        }

        [Fact]
        public void SaveBandMusicServiceTest()
        {

            var musicService = new MusicService(_context);
            var fullArtist = new FullArtist()
            {
                Id = "66CXWjxzNUsdJxJ2JdwvnR",
                Name = "Ariana Grande",
                Images = new List<Image> {
                    new Image() {
                        Url = "https://i.scdn.co/image/ab6761610000e5ebcdce7620dc940db079bf4952"
                    }
                }
            };
            musicService.SaveBand(fullArtist);

            var savedBand = _context.Bands.FirstOrDefault(b => b.BandId == "66CXWjxzNUsdJxJ2JdwvnR");

            Assert.NotNull(savedBand);
            Assert.Equal("66CXWjxzNUsdJxJ2JdwvnR", savedBand.BandId);
            Assert.Equal("Ariana Grande", savedBand.Name);
            Assert.Equal("https://i.scdn.co/image/ab6761610000e5ebcdce7620dc940db079bf4952", savedBand.Image);
        }

        [Fact]
        public void SaveSongMusicServiceTest()
        {
            var musicService = new MusicService(_context);

            var fullArtist = new FullArtist()
            {
                Id = "66CXWjxzNUsdJxJ2JdwvnR",
                Name = "Ariana Grande",
                Images = new List<Image> {
                    new Image() {
                        Url = "https://i.scdn.co/image/ab6761610000e5ebcdce7620dc940db079bf4952"
                    }
                }
            };
            musicService.SaveBand(fullArtist);

            var fullTrack = new FullTrack
            {
                Id = "6ocbgoVGwYJhOv1GgI9NsF",
                Name = "7 rings",
                Artists = new List<SimpleArtist> {
                    new SimpleArtist {
                        Id = "66CXWjxzNUsdJxJ2JdwvnR",
                        Name = "Ariana Grande"
                    }
                }
            };

            musicService.SaveSong(fullTrack);

            var savedSong = _context.Songs.FirstOrDefault(s => s.Name == "7 rings");
            Assert.NotNull(savedSong);
            Assert.Equal("7 rings", savedSong.Name);
            Assert.Equal("Ariana Grande", savedSong.Band.Name);
        }


        [Fact]
        public async void GetBandMusicServiceTest()
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
            var musicService = new MusicService(_context);
            var fullArtist = await apiSpotify.GetBandAsync("66CXWjxzNUsdJxJ2JdwvnR");
            musicService.SaveBand(fullArtist);
            var fullTrack = await apiSpotify.GetSongAsync("6ocbgoVGwYJhOv1GgI9NsF");
            musicService.SaveSong(fullTrack);
            var savedBand = _context.Bands.FirstOrDefault(b => b.BandId == "66CXWjxzNUsdJxJ2JdwvnR");
            Assert.NotNull(savedBand);
            Assert.Equal("66CXWjxzNUsdJxJ2JdwvnR", savedBand.BandId);
            Assert.Equal("Ariana Grande", savedBand.Name);
            Assert.Equal("https://i.scdn.co/image/ab6761610000e5ebcdce7620dc940db079bf4952", savedBand.Image);
            var savedSong = _context.Songs.FirstOrDefault(s => s.Name == "7 rings");
            Assert.NotNull(savedSong);
            Assert.Equal("7 rings", savedSong.Name);
            Assert.Equal("Ariana Grande", savedSong.Band.Name);
        }

        [Fact]
        public async void GetSongMusicServiceTest()
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
            var musicService = new MusicService(_context);
            var fullArtist = await apiSpotify.GetBandAsync("66CXWjxzNUsdJxJ2JdwvnR");
            musicService.SaveBand(fullArtist);
            var fullTrack = await apiSpotify.GetSongAsync("6ocbgoVGwYJhOv1GgI9NsF");
            musicService.SaveSong(fullTrack);
            var savedBand = _context.Bands.FirstOrDefault(b => b.BandId == "66CXWjxzNUsdJxJ2JdwvnR");
            Assert.NotNull(savedBand);
            Assert.Equal("66CXWjxzNUsdJxJ2JdwvnR", savedBand.BandId);
            Assert.Equal("Ariana Grande", savedBand.Name);
            Assert.Equal("https://i.scdn.co/image/ab6761610000e5ebcdce7620dc940db079bf4952", savedBand.Image);
            var savedSong = _context.Songs.FirstOrDefault(s => s.Name == "7 rings");
            Assert.NotNull(savedSong);
            Assert.Equal("7 rings", savedSong.Name);
            Assert.Equal("Ariana Grande", savedSong.Band.Name);
        }

    }

}