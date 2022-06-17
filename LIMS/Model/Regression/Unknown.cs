using LIMS.Enums;

namespace LIMS.Model.Regression
{
    public class Unknown : RegressionPoint
    {
        public override SampleType SampleType => SampleType.Unknown;
    }
}
