using System.ComponentModel.DataAnnotations;

namespace Tidbeat.Models {
    /// <summary>
    /// The band model.
    /// </summary>
    public class Band {
        /// <summary>
        /// The band id.
        /// </summary>
        [Key]
        public string BandId { get; set; }

        /// <summary>
        /// The band name.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// The band's image external URL.
        /// </summary>
        public string? Image { get; set; }

        /// <summary>
        /// The band's genre.
        /// </summary>
        public string? Gener { get; set; }
    }
}
