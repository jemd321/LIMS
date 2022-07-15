using LIMS.Enums;
using LIMS.Model;
using LIMS.Model.RegressionModels;
using System;

namespace LIMS.Factory
{
    public class RegressionFactory : IRegressionFactory
    {
        public Regression ConstructRegression(RegressionData regressionData, RegressionType regressionType = RegressionType.Linear, WeightingFactor weightingFactor = WeightingFactor.OneOverXSquared)
        {
            switch (regressionType)
            {
                case RegressionType.Linear:
                    return new LinearRegression(regressionData, weightingFactor);
                case RegressionType.Quadratic:
                    throw new NotImplementedException();
                default:
                    throw new Exception("invalid regressionType");
            }
        }
    }
}
