using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tidbeat.Models {
    /// <summary>
    /// The song model.
    /// </summary>
    public class Song {
        /// <summary>
        /// The song id.
        /// </summary>
        [Key]
        public string SongId { get; set; }

        /// <summary>
        /// The song name.
        /// </summary>
        [Required]
        public string Name { get; set;}

        /// <summary>
        /// The song's band.
        /// </summary>
        [Required]
        public Band Band { get; set; }

        /// <summary>
        /// The song's genre.
        /// </summary>
        public string? Gener { get; set; }
    }
}
