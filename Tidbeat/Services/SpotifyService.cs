using SpotifyAPI.Web;
using Tidbeat.Models;

namespace Tidbeat.Services {
    public class SpotifyService : ISpotifyService {
        //Add the id in the end of the url to get the song or band link to its page
        public const string UrlBand = "https://open.spotify.com/artist/";
        public const string UrlSong = "https://open.spotify.com/track/";

        private readonly ISpotifyClient _client;

        public SpotifyService(IConfiguration configuration) {
            string clientId = configuration["SpotifyAPI:ClientId"];
            string clientSecret = configuration["SpotifyAPI:ClientSecret"];
            _client = new SpotifyClient(SpotifyClientConfig.CreateDefault().WithAuthenticator(new ClientCredentialsAuthenticator(clientId, clientSecret)));
        }

        public async Task<FullTrack> GetSongAsync(string id) {
            var track = await _client.Tracks.Get(id);
            return track;
            //return new Song() { SongId = id, Name = track.Name, BandId = track.Artists[0].Id };
        }

        public async Task<FullArtist> GetBandAsync(string id) {
            var band = await _client.Artists.Get(id);
            return band;
            //return new Band() { BandId = id, Name = band.Name, Image = band.Images[0].Url };
        }
        public async Task<SearchResponse> GetMultipleBandsAsync(string searchKey)
        {
            SearchRequest searchTop;
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchTop = new SearchRequest(SearchRequest.Types.Artist, searchKey);
            }
            else
            {
                searchTop = new SearchRequest(SearchRequest.Types.Artist, "a");
            }
            var artist = await _client.Search.Item(searchTop);
            return artist;
        }
        public async Task<SearchResponse> GetSearchBandsbyValuesAsync(string searchKey, string gener) {
            var searchString = "";
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchString += searchKey;
            }
            if (!string.IsNullOrEmpty(gener))
            {
                searchString += "genre:" + gener;
            }
            SearchRequest searchTop = new SearchRequest(SearchRequest.Types.Artist, searchString);
            var bands = await _client.Search.Item(searchTop);
            return bands;
        }
    }
}
