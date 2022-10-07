using LIMS.Enums;

namespace LIMS.Model.RegressionModels
{
    /// <summary>
    /// A regression data point representing a quality control sample - to measure the performance of the assay.
    /// </summary>
    public class QualityControl : RegressionDataPoint
    {
        /// <summary>
        /// Gets the specific type of the sample.
        /// </summary>
        public override SampleType SampleType => SampleType.QualityControl;
    }
}
