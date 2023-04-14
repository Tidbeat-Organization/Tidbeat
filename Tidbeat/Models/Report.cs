using System.ComponentModel.DataAnnotations;
using Tidbeat.Enums;

namespace Tidbeat.Models
{
    public class Report
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public ReportReason Reason { get; set; }
        public string? DetailedReason { get; set; }

        [Required]
        public ApplicationUser UserReporter { get; set; }

        [Required]
        public string ReportItemId { get; set; }

        [Required]
        public ReportedItemType ReportItemType { get; set; }

        [Required]
        public ReportStatus Status { get; set; } = ReportStatus.Created;

        [Required]
        public DateTime Date { get; set; } = DateTime.Now;

        public ApplicationUser? ModAssigned{ get; set; }

    }
}
