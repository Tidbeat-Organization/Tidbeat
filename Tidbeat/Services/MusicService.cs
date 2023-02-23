using Tidbeat.Data;
using Tidbeat.Models;
using SpotifyAPI.Web;


namespace Tidbeat.Services {
    public class MusicService : IMusicService {
        private readonly ApplicationDbContext _context;

        public MusicService(ApplicationDbContext context) {
            _context = context;
        }

        public async Task<Band> GetBand(string bandId) {
            //FullArtist band = await _spotifyService.GetBandAsync(bandId);
            Band band = _context.Bands.Find(bandId);
            if (band == null) {
                return new Band() { Name = "Not found", Image = "" };
            }
            return band;
        }

        public async Task<Song> GetSong(string songId) {
            //FullTrack song = await _spotifyService.GetSongAsync(songId);
            Song song = _context.Songs.Find(songId);
            if (song == null) {
                return new Song() { Name = "Not found", BandId = "" };
            }
            return song;
        }

        public void SaveBand(FullArtist band) {
            _context.Bands.Add(new Band() { BandId = band.Id, Name = band.Name, Image = band.Images[0].Url });
            _context.SaveChanges();
        }

        public void SaveSong(FullTrack song) {
            _context.Songs.Add(new Song() { SongId = song.Id, Name = song.Name, BandId = song.Artists[0].Id });
            _context.SaveChanges();
        }
    }
}
