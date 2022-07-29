using LIMS.Enums;

namespace LIMS.Model.RegressionModels
{
    public abstract class Regression
    {
        public RegressionData RegressionData { get; private protected set; }
        public WeightingFactor WeightingFactor { get; private protected set; }
        public double? Gradient { get; private protected set; }
        public double? YIntercept { get; private protected set; }

        public abstract void UpdateRegression();
    }
}