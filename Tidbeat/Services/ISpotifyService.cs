using Tidbeat.Models;

namespace Tidbeat.Services {
    public interface ISpotifyService {
        Task<Song> GetSongAsync(string id);

        Task<Band> GetBandAsync(string id);
    }
}
