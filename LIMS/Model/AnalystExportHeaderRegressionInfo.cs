using LIMS.Enums;

namespace LIMS.Model
{
    public readonly record struct AnalystExportHeaderRegressionInfo
    {
        public RegressionType Regression { get; init; }

        public WeightingFactor WeightingFactor { get; init; }

        public double? A { get; init; }

        public double? B { get; init; }

        public double? C { get; init; }

        public double RSquared { get; init; }
    }
}
