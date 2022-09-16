using LIMS.Enums;

namespace LIMS.Model.RegressionModels
{
    /// <summary>
    /// A class representing a calibration standard for use in a regression.
    /// </summary>
    public class Standard : RegressionDataPoint
    {
        /// <summary>
        /// Gets the sample type, which is a calibration standard.
        /// </summary>
        public override SampleType SampleType => SampleType.Standard;
    }
}
