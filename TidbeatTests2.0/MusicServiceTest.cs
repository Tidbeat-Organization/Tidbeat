using SpotifyAPI.Web;
using Tidbeat.Data;
using Tidbeat.Services;

namespace TidbeatTests2._0 {
    public class MusicServiceTest : IClassFixture<ApplicationDbContextFixture> {
        private ApplicationDbContext _context;

        public MusicServiceTest(ApplicationDbContextFixture fixture) {
            _context = fixture.ApplicationDbContext;
        }

        [Fact]
        public void SaveBandMusicServiceTest() {

            var musicService = new MusicService(_context);
            var fullArtist = new FullArtist() {
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
        public void SaveSongMusicServiceTest() {
            var musicService = new MusicService(_context);

            var fullArtist = new FullArtist() {
                Id = "2HHmvvSQ44ePDH7IKVzgK0",
                Name = "Jain",
                Images = new List<Image> {
                    new Image() {
                        Url = "https://i.scdn.co/image/ab6761610000e5eb673f287fead1f6c83b9b68ea"
                    }
                }
            };
            musicService.SaveBand(fullArtist);

            var fullTrack = new FullTrack {
                Id = "5JdLUE9D743ob2RtgmVpVx",
                Name = "Makeba",
                Artists = new List<SimpleArtist> {
                    new SimpleArtist {
                        Id = "2HHmvvSQ44ePDH7IKVzgK0",
                        Name = "Jain"
                    }
                }
            };

            musicService.SaveSong(fullTrack);

            var savedSong = _context.Songs.FirstOrDefault(s => s.Name == "Makeba");
            Assert.NotNull(savedSong);
            Assert.Equal("Makeba", savedSong.Name);
            Assert.Equal("Jain", savedSong.Band.Name);
        }
    }

}