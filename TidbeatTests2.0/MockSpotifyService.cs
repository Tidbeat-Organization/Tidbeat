using Moq;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tidbeat.Services;

namespace TidbeatTests2._0
{
    public class MockSpotifyService : ISpotifyService
    {
        private readonly Mock<ISpotifyService> _mock;

        public MockSpotifyService()
        {
            _mock = new Mock<ISpotifyService>();
        }

        public async Task<int?> GetAmountBandAlbumAsync(string id)
        {
            return await Task.FromResult(0);
        }

        public async Task<FullArtist> GetBandAsync(string id)
        {
            return await Task.FromResult(new FullArtist { Id = id, Name = "Test Artist" });
        }

        public async Task<SearchResponse> GetMultipleBandsAsync(string searchKey)
        {
            SearchResponse response = new SearchResponse();
            return await Task.FromResult(response);
        }

        public async Task<SearchResponse> GetMultipleSongsAsync(string searchKey)
        {
            SearchResponse response = new SearchResponse();
            return await Task.FromResult(response);
        }

        public async Task<SearchResponse> GetSearchBandsbyValuesAsync(string searchKey, string gener)
        {
            SearchResponse response = new SearchResponse();
            return await Task.FromResult(response);
        }

        public async Task<SearchResponse> GetSearchSongsbyValuesAsync(string Search, string Gener, string band, string album, string yearStart, string yearEnd)
        {
           /* var track1 = new FullTrack
            {
                Id = "1",
                Name = "Test Song"
            };
            track1.Artists.Add(new SimpleArtist
            {
                Name = "Artist Name",
                Id = "06HL4z0CvFAxyc27GXpf02"
            });
            track1.Images.Add(new Image
            {
                Url = "https://i.scdn.co/image/ab67616d0000b273bb54dde68cd23e2a268ae0f5"
            });

            var track2 = new FullTrack
            {
                Id = "2",
                Name = "Test Song"
            };
            track2.Artists.Add(new SimpleArtist
            {
                Name = "Artist Name",
                Id = "06HL4z0CvFAxyc27GXpf02"
            });
            track2.Images.Add(new Image
            {
                Url = "https://i.scdn.co/image/ab67616d0000b273bb54dde68cd23e2a268ae0f5"
            });
            List<FullTrack> result = new List<FullTrack>();
            result.Add(track1);
            result.Add(track2);*/
            SearchResponse response = new SearchResponse();
            return await Task.FromResult(response);
        }

        public async Task<FullTrack> GetSongAsync(string id)
        {
            var track = new FullTrack
            {
                Id = id,
                Name = "Test Song"
            };
            track.Artists.Add(new SimpleArtist
            {
                Name = "Artist Name",
                Id = "66CXWjxzNUsdJxJ2JdwvnR"
            });
            return await Task.FromResult(track);
        }

        public async Task<List<FullTrack>> GetTop3SongsAsync(string artistId)
        {
            var track1 = new FullTrack
            {
                Id = artistId,
                Name = "Test Song1"
            };
            track1.Artists.Add(new SimpleArtist
            {
                Name = "Artist Name1",
                Id = artistId
            });
            var track2 = new FullTrack
            {
                Id = artistId,
                Name = "Test Song2"
            };
            track2.Artists.Add(new SimpleArtist
            {
                Name = "Artist Name2",
                Id = artistId
            });
            var track3 = new FullTrack
            {
                Id = artistId,
                Name = "Test Song3"
            };
            track2.Artists.Add(new SimpleArtist
            {
                Name = "Artist Name3",
                Id = artistId
            });
            List<FullTrack> result = new List<FullTrack>();
            result.Add(track1);
            result.Add(track2);
            result.Add(track3);
            return await Task.FromResult(result);
        }

    }
}
