using LIMS.Enums;
using LIMS.Model;
using LIMS.Model.RegressionModels;

namespace LIMS.Factory
{
    public interface IRegressionFactory
    {
        public Regression ConstructRegression(RegressionData regressionData, RegressionType regressionType = RegressionType.Linear, WeightingFactor weightingFactor = WeightingFactor.OneOverXSquared);
    }
}
