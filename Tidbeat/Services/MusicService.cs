using Tidbeat.Data;
using Tidbeat.Models;
using SpotifyAPI.Web;

namespace Tidbeat.Services {
    /// <summary>
    /// The music service. It gets and saves songs and bands in the database.
    /// </summary>
    public class MusicService : IMusicService {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes the music service.
        /// </summary>
        /// <param name="context">The context of the application.</param>
        public MusicService(ApplicationDbContext context) {
            _context = context;
        }

        /// <summary>
        /// Gets a band from the database.
        /// </summary>
        /// <param name="bandId">The id of the band.</param>
        /// <returns>The band.</returns>
        public async Task<Band> GetBand(string bandId) {
            //FullArtist band = await _spotifyService.GetBandAsync(bandId);
            Band band = _context.Bands.Find(bandId);
            if (band == null) {
                return new Band() { Name = "Not found", Image = "" };
            }
            return band;
        }

        /// <summary>
        /// Gets a song from the database.
        /// </summary>
        /// <param name="songId">The id of the song.</param>
        /// <returns>The song.</returns>
        public async Task<Song> GetSong(string songId) {
            //FullTrack song = await _spotifyService.GetSongAsync(songId);
            Song song = _context.Songs.Find(songId);
            if (song == null) {
                return new Song() { Name = "Not found", Band = new Band() };
            }
            return song;
        }

        /// <summary>
        /// Saves a band in the database.
        /// </summary>
        /// <param name="band">The band to save.</param>
        public void SaveBand(FullArtist band) {
            _context.Bands.Add(new Band() { BandId = band.Id, Name = band.Name, Image = band.Images[0].Url });
            _context.SaveChanges();
        }

        /// <summary>
        /// Saves a song in the database.
        /// </summary>
        /// <param name="song">The song to save.</param>
        public void SaveSong(FullTrack song) {
            var artist = song.Artists[0];
            _context.Songs.Add(new Song() { SongId = song.Id, Name = song.Name, Band = _context.Bands.Find(artist.Id) });
            _context.SaveChanges();
        }

        /// <summary>
        /// Gets all the genres.
        /// </summary>
        /// <returns>A list of all the genres.</returns>
        public static List<string> AllGenres() {
            List<string> values = new List<string>() { "rock", "pop", "kids", "funk", "classical", "country", "dance", "metal", "disco", "folk", "hip-hop", "indian", "k-pop", "punk", "piano", "reggae", "techno" };
            return values;
        }
    }
}
