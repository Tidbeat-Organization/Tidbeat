using Tidbeat.Models;
using SpotifyAPI.Web;


namespace Tidbeat.Services {
    public interface IMusicService {
        Task<Song> GetSong(string songId);
        void SaveSong(FullTrack song);
        Task<Band> GetBand(string bandId);
        void SaveBand(FullArtist band);
    }
}
