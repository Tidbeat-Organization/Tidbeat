using Tidbeat.Models;
using SpotifyAPI.Web;

namespace Tidbeat.Services {
    public interface ISpotifyService {
        Task<FullTrack> GetSongAsync(string id);

        Task<FullArtist> GetBandAsync(string id);

        Task<SearchResponse> GetMultipleSongsAsync();
        Task<SearchResponse> GetSearchSongsbyValuesAsync(string Gener, string band, string album, string yearStart, string yearEnd);
    }
}
