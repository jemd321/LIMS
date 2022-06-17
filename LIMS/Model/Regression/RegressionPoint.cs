using LIMS.Enums;

namespace LIMS.Model.Regression
{
    public abstract class RegressionPoint
    {
        // Concentration
        public double X { get; init; }
        // Instrument Response
        public double? Y { get; init; }

        public bool IsActive { get; set; } = true;
        public string SampleName { get; init; }
        public abstract SampleType SampleType { get; }
    }
}
