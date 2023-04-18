using Tidbeat.Enums;

namespace Tidbeat.DTOs.Reports {
    public class SetReportStatusDTO {
        public string ReportId { get; set; }
        public int Status { get; set; }
    }
}
