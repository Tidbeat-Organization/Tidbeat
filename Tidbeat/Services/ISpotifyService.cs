using Tidbeat.Models;
using SpotifyAPI.Web;
using System.Diagnostics.Metrics;

namespace Tidbeat.Services {
    /// <summary>
    /// The Spotify service. Takes care of all the operations related to getting info from the Spotify API.
    /// </summary>
    public interface ISpotifyService {
        /// <summary>
        /// Gets a song from the Spotify API.
        /// </summary>
        /// <param name="id">The id of the song.</param>
        /// <returns>The spotify FullTrack object.</returns>
        Task<FullTrack> GetSongAsync(string id);

        /// <summary>
        /// Gets a song's genres from the Spotify API.
        /// </summary>
        /// <param name="id">The id of the song.</param>
        /// <returns>The string array with the results.</returns>
        Task<List<string>> GetGenresOfSong(string id);

        /// <summary>
        /// Gets a band's genres from the Spotify API.
        /// </summary>
        /// <param name="id">The id of the band.</param>
        /// <returns>The string array with the results.</returns>
        Task<List<string>> GetGenresOfBand(string id);

        Task<bool> SongHasGenre(string id, string genre);

        Task<bool> BandHasGenre(string id, string genre);

        
        /// <summary>
        /// Gets a band from the Spotify API.
        /// </summary>
        /// <param name="id">The id of the band.</param>
        /// <returns>The spotify FullArtist object.</returns>
        Task<FullArtist> GetBandAsync(string id);

        /// <summary>
        /// Gets the amount of albums a band has.
        /// </summary>
        /// <param name="id">The id of the band.</param>
        /// <returns>The amount of albums the band has.</returns>
        Task<int?> GetAmountBandAlbumAsync(string id);

        /// <summary>
        /// Gets the top 3 songs of a band.
        /// </summary>
        /// <param name="artistId">The id of the band.</param>
        /// <returns>A list of the top 3 songs of the band.</returns>
        Task<List<FullTrack>> GetTop3SongsAsync(string artistId);

        /// <summary>
        /// Searches for bands using a search key.
        /// </summary>
        /// <param name="searchKey">The search key.</param>
        /// <returns>A list of the bands that match the search key.</returns>
        Task<SearchResponse> GetMultipleBandsAsync(string searchKey);

        /// <summary>
        /// Searches for bands using a search key and a genre.
        /// </summary>
        /// <param name="searchKey">The search key.</param>
        /// <param name="gener">The genre of the band.</param>
        /// <returns>A list of the bands that match the search key and genre.</returns>
        Task<SearchResponse> GetSearchBandsbyValuesAsync(string searchKey, string gener);

        /// <summary>
        /// Searches for songs using a search key
        /// </summary>
        /// <param name="searchKey">The search key.</param>
        /// <returns>A list of the songs that match the search key.</returns>
        Task<SearchResponse> GetMultipleSongsAsync(string searchKey);

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
        Task<SearchResponse> GetSearchSongsbyValuesAsync(string Search,string Gener, string band, string album, string yearStart, string yearEnd);
    }
}
