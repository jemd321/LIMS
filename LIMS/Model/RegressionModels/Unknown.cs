using LIMS.Enums;

namespace LIMS.Model.RegressionModels
{
    public class Unknown : RegressionDataPoint
    {
        public override SampleType SampleType => SampleType.Unknown;
    }
}
