using SpotifyAPI.Web;
using Tidbeat.Data;
using Tidbeat.Services;

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
        public void GetBandMusicServiceTest()
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
    }

}