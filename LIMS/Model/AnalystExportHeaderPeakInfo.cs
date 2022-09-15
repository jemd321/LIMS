namespace LIMS.Model
{
    /// <summary>
    /// The information from the header section of an Analyst sofware export, describing exactly one analyte peak.
    /// </summary>
    public readonly record struct AnalystExportHeaderPeakInfo
    {
        /// <summary>
        /// Gets the name of the analyte that generated the peak.
        /// </summary>
        public string PeakName { get; init; }

        /// <summary>
        /// Gets a value indicating whether the analyte is used as an internal standard in the assay.
        /// </summary>
        public bool IsInternalStandard { get; init; }

        /// <summary>
        /// Gets the name of the peak  which this analyte is acting as an internal standard for, if doing so.
        /// </summary>
        public string InternalStandard { get; init; }

        /// <summary>
        /// Gets the mass spectrometry transition used to generate the instrument response for this peak.
        /// </summary>
        public TransitionMRM TransitionMRM { get; init; }
    }
}
