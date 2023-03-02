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

        public async Task<FullTrack> GetSongAsync(string id) 
        {
            var track = await _client.Tracks.Get(id);
            return track;
            //return new Song() { SongId = id, Name = track.Name, BandId = track.Artists[0].Id };
        }

        public async Task<FullArtist> GetBandAsync(string id) 
        {
            var band = await _client.Artists.Get(id);
            return band;
            //return new Band() { BandId = id, Name = band.Name, Image = band.Images[0].Url };
        } 

        public async Task<int?> GetAmountBandAlbumAsync(string id) {
            var albums = await _client.Artists.GetAlbums(id);
            //return albums.Items.Where(a => a.Artists.Any(ar => ar.Id == id) && a.AlbumType == "album" && a.ReleaseDate != null && a.ReleaseDatePrecision == "day").ToList().Count;
            return albums.Items?.Count(album => album.AlbumType == "album");
        }

        public async Task<List<FullTrack>> GetTop3SongsAsync(string artistId) {
            var request = new ArtistsTopTracksRequest(artistId);
            var topTracks = await _client.Artists.GetTopTracks(artistId, new ArtistsTopTracksRequest("US"));
            var sortedTracks = topTracks.Tracks.OrderByDescending(t => t.Popularity);

            return sortedTracks.Take(3).ToList();
        }
      public async Task<SearchResponse> GetMultipleSongsAsync(string searchKey) 
        {
            SearchRequest searchTop;
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchTop = new SearchRequest(SearchRequest.Types.Track, searchKey);
            }
            else {
                searchTop = new SearchRequest(SearchRequest.Types.Track, "a");
            }
            var tracks = await _client.Search.Item(searchTop);
            return tracks;
        }

        public async Task<SearchResponse> GetSearchSongsbyValuesAsync(string Search,string Gener, string band, string album, string yearStart, string yearEnd) 
        {
            var searchString = "";
            if (!string.IsNullOrEmpty(Search))
            {
                searchString += Search;
            }
            if (!string.IsNullOrEmpty(Gener)){
                searchString += " genre:" + Gener;
            }
            if (!string.IsNullOrEmpty(band))
            {
                searchString += " artist:" + band;
            }
            if (!string.IsNullOrEmpty(album))
            {
                searchString += " album:" + album;
            }
            if (!string.IsNullOrEmpty(yearStart) && yearStart.Length == 4)
            {
                Console.WriteLine("YEAR START IS NOT NULL");
                if (string.IsNullOrEmpty(yearEnd) || yearEnd.Length == 4)
                {
                    searchString += " year:" + yearStart + "-" + yearStart;
                }
                else
                {
                    searchString += " year:" + yearStart + "-" + yearEnd;
                }
            }
            else if (!string.IsNullOrEmpty(yearEnd) && yearEnd.Length == 4)
            {
                Console.WriteLine("YEAR End IS NOT NULL");
                if (string.IsNullOrEmpty(yearStart) || yearStart.Length == 4)
                {
                    searchString += " year:" + yearEnd + "-" + yearEnd;
                }
                else
                {
                    searchString += " year:" + yearStart + "-" + yearEnd;
                }
            }
            SearchRequest searchTop = new SearchRequest(SearchRequest.Types.Track, searchString);
            var tracks = await _client.Search.Item(searchTop);
            return tracks;
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
                searchString += " genre:" + gener;
            }
            if (string.IsNullOrEmpty(searchString))
            {
                return await GetMultipleBandsAsync(searchKey);
            }
            else 
            {
                SearchRequest searchTop = new SearchRequest(SearchRequest.Types.Artist, searchString);
                var bands = await _client.Search.Item(searchTop);
                return bands;
            }
            return await GetMultipleBandsAsync(searchKey);
        }
    }
}