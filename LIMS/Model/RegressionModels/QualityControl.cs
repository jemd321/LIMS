using LIMS.Enums;

namespace LIMS.Model.RegressionModels
{
    public class QualityControl : RegressionDataPoint
    {
        public override SampleType SampleType => SampleType.QualityControl;
    }
}
