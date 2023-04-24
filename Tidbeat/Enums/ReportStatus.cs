namespace Tidbeat.Enums
{
    /// <summary>
    /// The status of the report.
    /// </summary>
    public enum ReportStatus
    {
        /// <summary>
        /// The report has been created, but not opened nor closed.
        /// </summary>
        Created,
        /// <summary>
        /// The report is created and open for analysis.
        /// </summary>
        Open,
        /// <summary>
        /// The report is created and closed.
        /// </summary>
        Closed
    }
}
