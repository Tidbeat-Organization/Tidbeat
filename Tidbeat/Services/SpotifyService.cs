using SpotifyAPI.Web;
using Tidbeat.Models;

namespace Tidbeat.Services {
    /// <summary>
    /// The Spotify service. Takes care of all the operations related to getting info from the Spotify API.
    /// </summary>
    public class SpotifyService : ISpotifyService {
        //Add the id in the end of the url to get the song or band link to its page
        public const string UrlBand = "https://open.spotify.com/artist/";
        public const string UrlSong = "https://open.spotify.com/track/";

        private readonly ISpotifyClient _client;

        /// <summary>
        /// Initializes the Spotify service.
        /// </summary>
        /// <param name="configuration">The configuration of the application.</param>
        public SpotifyService(IConfiguration configuration) {
            string clientId = configuration["SpotifyAPI:ClientId"];
            string clientSecret = configuration["SpotifyAPI:ClientSecret"];
            _client = new SpotifyClient(SpotifyClientConfig.CreateDefault().WithAuthenticator(new ClientCredentialsAuthenticator(clientId, clientSecret)));
        }

        /// <summary>
        /// Gets a song from the Spotify API.
        /// </summary>
        /// <param name="id">The id of the song.</param>
        /// <returns>The spotify FullTrack object.</returns>
        public async Task<FullTrack> GetSongAsync(string id) 
        {
            try {                 
                var track = await _client.Tracks.Get(id);
                return track;
            }
            catch (APIException e) {
                return null;
            }
            //return new Song() { SongId = id, Name = track.Name, BandId = track.Artists[0].Id };
        }

        /// <summary>
        /// Gets a song's genres from the Spotify API.
        /// </summary>
        /// <param name="id">The id of the song.</param>
        /// <returns>The string array with the results.</returns>
        public async Task<List<string>> GetGenresOfSong(string id)
        {
            try
            {
                var track = await _client.Tracks.Get(id);
                //Console.WriteLine($"[ Found track named {track.Name} of ID: {track.Id}]");
                var album = await _client.Albums.Get(track.Album.Id);
                //Console.WriteLine($"[ Found album named {album.Name} with genres: ]");
                //foreach (var genre in album.Genres)
                //{
                //    Console.WriteLine($"[ -- ${genre} ]");
                //}
                //Console.WriteLine($"[ Count of genres: {album.Genres.Count} ]");
                return album.Genres.ToList();
            }
            catch (APIException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        /// <summary>
        /// Gets a band's genres from the Spotify API.
        /// </summary>
        /// <param name="id">The id of the band.</param>
        /// <returns>The string array with the results.</returns>
        public async Task<List<string>> GetGenresOfBand(string id)
        {
            try
            {
                var band = await _client.Artists.Get(id);
                return band.Genres.ToList();
            }
            catch (APIException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        
        public async Task<bool> SongHasGenre(string id, string genre)
        {
            try
            {
                var track = await _client.Tracks.Get(id);
                var album = await _client.Albums.Get(track.Album.Id);
                
                return album.Genres.Any(g => g.Contains(genre));
            }
            catch (APIException e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public async Task<bool> BandHasGenre(string id, string genre)
        {
            try
            {
                var band = await _client.Artists.Get(id);
                
                return band.Genres.Any(g => g.Contains(genre));
            }
            catch (APIException e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Gets a band from the Spotify API.
        /// </summary>
        /// <param name="id">The id of the band.</param>
        /// <returns>The spotify FullArtist object.</returns>
        public async Task<FullArtist> GetBandAsync(string id) 
        {
            try {
                var band = await _client.Artists.Get(id);
                return band;
            }
            catch (APIException e) {
                return null;
            }
            //return new Band() { BandId = id, Name = band.Name, Image = band.Images[0].Url };
        } 

        /// <summary>
        /// Gets the amount of albums a band has.
        /// </summary>
        /// <param name="id">The id of the band.</param>
        /// <returns>The amount of albums the band has.</returns>
        public async Task<int?> GetAmountBandAlbumAsync(string id) {
            var albums = await _client.Artists.GetAlbums(id);
            //return albums.Items.Where(a => a.Artists.Any(ar => ar.Id == id) && a.AlbumType == "album" && a.ReleaseDate != null && a.ReleaseDatePrecision == "day").ToList().Count;
            return albums.Items?.Count(album => album.AlbumType == "album");
        }

        /// <summary>
        /// Gets the top 3 songs of a band.
        /// </summary>
        /// <param name="artistId">The id of the band.</param>
        /// <returns>A list of the top 3 songs of the band.</returns>
        public async Task<List<FullTrack>> GetTop3SongsAsync(string artistId) {
            var request = new ArtistsTopTracksRequest(artistId);
            var topTracks = await _client.Artists.GetTopTracks(artistId, new ArtistsTopTracksRequest("US"));
            var sortedTracks = topTracks.Tracks.OrderByDescending(t => t.Popularity);

            return sortedTracks.Take(3).ToList();
        }

        /// <summary>
        /// Searches for songs using a search key
        /// </summary>
        /// <param name="searchKey">The search key.</param>
        /// <returns>A list of the songs that match the search key.</returns>
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

        /// <summary>
        /// Searches for songs using a search key and filters.
        /// </summary>
        /// <param name="Search">The search key.</param>
        /// <param name="Gener">The genre of the songs.</param>
        /// <param name="band">The band of the songs.</param>
        /// <param name="album">The album of the songs.</param>
        /// <param name="yearStart">The start year of the songs.</param>
        /// <param name="yearEnd">The end year of the songs.</param>
        /// <returns>A list of the songs that match the search key and filters.</returns>
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
                if (string.IsNullOrEmpty(yearEnd) || yearEnd.Length != 4)
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
                if (string.IsNullOrEmpty(yearStart) || yearStart.Length != 4)
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

        /// <summary>
        /// Searches for bands using a search key.
        /// </summary>
        /// <param name="searchKey">The search key.</param>
        /// <returns>A list of the bands that match the search key.</returns>
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

        /// <summary>
        /// Searches for bands using a search key and a genre.
        /// </summary>
        /// <param name="searchKey">The search key.</param>
        /// <param name="gener">The genre of the band.</param>
        /// <returns>A list of the bands that match the search key and genre.</returns>
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