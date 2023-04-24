using Tidbeat.Enums;

namespace Tidbeat.DTOs.Reports {
    /// <summary>
    /// The data transfer object for the SetReportStatus method in the ReportsController.
    /// </summary>
    public class SetReportStatusDTO {
        /// <summary>
        /// The id of the report.
        /// </summary>
        public string ReportId { get; set; }

        /// <summary>
        /// The new status of the report.
        /// </summary>
        public int Status { get; set; }
    }
}
