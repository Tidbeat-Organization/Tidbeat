using Tidbeat.Models;
using SpotifyAPI.Web;
using System.Diagnostics.Metrics;

namespace Tidbeat.Services {
    public interface ISpotifyService {
        Task<FullTrack> GetSongAsync(string id);

        Task<FullArtist> GetBandAsync(string id);

        Task<SearchResponse> GetMultipleBandsAsync(string searchKey);
        Task<SearchResponse> GetSearchBandsbyValuesAsync(string searchKey, string gener);
    }
}
