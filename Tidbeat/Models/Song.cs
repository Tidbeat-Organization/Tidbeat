using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tidbeat.Models {
    public class Song {
        [Key]
        public string SongId { get; set; }

        [Required]
        public string Name { get; set;}

        [ForeignKey("Band")]
        public string BandId { get; set; }
    }
}
