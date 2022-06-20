using LIMS.Enums;

namespace LIMS.Model.RegressionModels
{
    public abstract class RegressionDataPoint
    {

        // Concentration
        public double? X { get; init; }
        // Instrument Response
        public double? Y { get; init; }

        public bool IsActive { get; set; } = true;
        public string SampleName { get; init; }
        public abstract SampleType SampleType { get; }
    }
}
