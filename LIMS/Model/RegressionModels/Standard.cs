using LIMS.Enums;

namespace LIMS.Model.RegressionModels
{
    public class Standard : RegressionDataPoint
    {
        public override SampleType SampleType => SampleType.Standard;
    }
}
