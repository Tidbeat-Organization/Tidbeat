using Tidbeat.Models;

namespace Tidbeat.Services {
    public interface IMusicService {
        Task<Song> GetSong(string songId);
        void SaveSong(Song song);
        Task<Band> GetBand(string bandId);
        void SaveBand(Band band);
    }
}
