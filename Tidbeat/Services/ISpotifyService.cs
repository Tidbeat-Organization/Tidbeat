using Tidbeat.Models;
using SpotifyAPI.Web;

namespace Tidbeat.Services {
    public interface ISpotifyService {
        Task<FullTrack> GetSongAsync(string id);

        Task<FullArtist> GetBandAsync(string id);

        Task<int?> GetAmountBandAlbumAsync(string id);

        Task<List<FullTrack>> GetTop3SongsAsync(string artistId);
    }
}
