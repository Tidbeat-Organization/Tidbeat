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
    }
}
