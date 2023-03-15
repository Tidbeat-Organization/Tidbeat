using Tidbeat.Models;
using SpotifyAPI.Web;
using System.Diagnostics.Metrics;

namespace Tidbeat.Services {
    public interface ISpotifyService {
        Task<FullTrack> GetSongAsync(string id);

        Task<FullArtist> GetBandAsync(string id);

        Task<int?> GetAmountBandAlbumAsync(string id);
        Task<List<FullTrack>> GetTop3SongsAsync(string artistId);
        Task<SearchResponse> GetMultipleBandsAsync(string searchKey);
        Task<SearchResponse> GetSearchBandsbyValuesAsync(string searchKey, string gener);
        Task<SearchResponse> GetMultipleSongsAsync(string searchKey);
        Task<SearchResponse> GetSearchSongsbyValuesAsync(string Search,string Gener, string band, string album, string yearStart, string yearEnd);
    }
}
