using System.ComponentModel.DataAnnotations;

namespace Tidbeat.Models
{
    /// <summary>
    /// The ban details for a user.
    /// </summary>
    public class BanUser
    {
        /// <summary>
        /// The id of the ban.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The user which was just banned.
        /// </summary>
        [Required]
        public ApplicationUser User { get; set; }

        /// <summary>
        /// The reason of the ban.
        /// </summary>
        public string? Reason { get; set; }

        /// <summary>
        /// The finish date of the ban.
        /// </summary>
        [Required]
        public DateTime EndsAt { get; set; }

    }
}
