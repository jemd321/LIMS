using LIMS.Enums;

namespace LIMS.Model.RegressionModels
{
    public abstract class RegressionDataPoint
    {

        // Concentration
        public double? CalculatedConcentration { get; init; }
        // Instrument Response
        public double? InstrumentResponse { get; init; }
        public double? NominalConcentration { get; init; }

        public bool IsActive { get; set; } = true;
        public string SampleName { get; init; }
        public abstract SampleType SampleType { get; }
    }
}
