using System.ComponentModel.DataAnnotations;
using Tidbeat.Enums;

namespace Tidbeat.Models
{
    /// <summary>
    /// The report for a specific item.
    /// </summary>
    public class Report
    {
        /// <summary>
        /// The id of the report.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// The reason of the report.
        /// </summary>
        [Required]
        public ReportReason? Reason { get; set; }

        /// <summary>
        /// The detailed reason of the report.
        /// </summary>
        public string? DetailedReason { get; set; }

        /// <summary>
        /// The user that reported another item.
        /// </summary>
        [Required]
        public ApplicationUser UserReporter { get; set; }

        /// <summary>
        /// The user that was reported by another user.
        /// </summary>
        [Required]
        public ApplicationUser UserReported { get; set; }

        /// <summary>
        /// The id of the item reported.
        /// </summary>
        public string? ReportItemId { get; set; }

        /// <summary>
        /// The type of the reported item.
        /// </summary>
        public ReportedItemType? ReportItemType { get; set; }

        /// <summary>
        /// The status type of the report.
        /// </summary>
        [Required]
        public ReportStatus Status { get; set; } = ReportStatus.Created;

        /// <summary>
        /// The date of the report.
        /// </summary>
        [Required]
        public DateTime Date { get; set; } = DateTime.Now;

        /// <summary>
        /// The assigned mod. NOT BEING USED RIGHT NOW.
        /// </summary>
        public ApplicationUser? ModAssigned{ get; set; }

    }
}
