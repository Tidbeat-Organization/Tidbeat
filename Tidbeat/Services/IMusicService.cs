using Tidbeat.Models;
using SpotifyAPI.Web;


namespace Tidbeat.Services {
    /// <summary>
    /// The music service. It gets and saves songs and bands in the database.
    /// </summary>
    public interface IMusicService {
        /// <summary>
        /// Gets a song from the database.
        /// </summary>
        /// <param name="songId">The id of the song.</param>
        /// <returns>The song.</returns>
        Task<Song> GetSong(string songId);

        /// <summary>
        /// Saves a song in the database.
        /// </summary>
        /// <param name="song">The song to save.</param>
        void SaveSong(FullTrack song);

        /// <summary>
        /// Gets a band from the database.
        /// </summary>
        /// <param name="bandId">The id of the band.</param>
        /// <returns>The band.</returns>
        Task<Band> GetBand(string bandId);

        /// <summary>
        /// Saves a band in the database.
        /// </summary>
        /// <param name="band">The band to save.</param>
        void SaveBand(FullArtist band);
    }
}
