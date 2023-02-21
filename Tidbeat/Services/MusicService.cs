using Tidbeat.Data;
using Tidbeat.Models;

namespace Tidbeat.Services {
    public class MusicService : IMusicService {
        private readonly ISpotifyService _spotifyService;
        private readonly ApplicationDbContext _context;

        public MusicService(ISpotifyService spotifyService, ApplicationDbContext context) {
            _spotifyService = spotifyService;
            _context = context;
        }

        public async Task<Band> GetBand(string bandId) {
            Band band = await _spotifyService.GetBandAsync(bandId);
            return band;
        }

        public async Task<Song> GetSong(string songId) {
            Song song = await _spotifyService.GetSongAsync(songId);
            return song;
        }

        public void SaveBand(Band band) {
            _context.Bands.Add(band);
            _context.SaveChanges();
        }

        public void SaveSong(Song song) {
            _context.Songs.Add(song);
            _context.SaveChanges();
        }
    }
}
