using LIMS.Enums;

namespace LIMS.Model.Regression
{
    public class Standard : RegressionPoint
    {
        public override SampleType SampleType => SampleType.Standard;
    }
}
