namespace LIMS.Enums
{
    /// <summary>
    /// Describes the category that every sample (ie. one period of acquisition by the instrument) must fall into.
    /// </summary>
    public enum SampleType
    {
        /// <summary>
        /// The concentration is unknown - either because it is blank or a test sample that needs to be measured.
        /// </summary>
        Unknown,

        /// <summary>
        /// A calibration standard prepared at a known concentration, which will be used in the regression.
        /// </summary>
        Standard,

        /// <summary>
        /// A quality control sample, prepared at a known concentration and measured against the calibration line to validate the assay.
        /// </summary>
        QualityControl,
    }
}
