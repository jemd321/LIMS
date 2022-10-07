using LIMS.Enums;

namespace LIMS.Model.RegressionModels
{
    /// <summary>
    /// A class representing an unknown sample in the regression, which may be a blank or a test sample.
    /// </summary>
    public class Unknown : RegressionDataPoint
    {
        /// <summary>
        /// Gets type of the sample.
        /// </summary>
        public override SampleType SampleType => SampleType.Unknown;
    }
}
