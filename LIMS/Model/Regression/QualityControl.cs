using LIMS.Enums;

namespace LIMS.Model.Regression
{
    public class QualityControl : RegressionPoint
    {
        public override SampleType SampleType => SampleType.QualityControl;
    }
}
