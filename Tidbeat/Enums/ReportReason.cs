namespace Tidbeat.Enums
{
    /// <summary>
    /// The reason for reporting an item.
    /// </summary>
    public enum ReportReason
    {
        /// <summary>
        /// Other types not mentioned.
        /// </summary>
        Other,
        /// <summary>
        /// There is hate speech.
        /// </summary>
        HateSpeech,
        /// <summary>
        /// There is sexual content or mentions of it.
        /// </summary>
        SexualContent,
        /// <summary>
        /// There is gore content or mentions of it.
        /// </summary>
        GoreContent,
        /// <summary>
        /// The behaviour is considered innappropriate for the situation.
        /// </summary>
        InnappropriateBehaviour
    }
}
