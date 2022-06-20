using LIMS.Enums;
using LIMS.Model;
using LIMS.Model.RegressionModels;
using System;

namespace LIMS.Factory
{
    public class RegressionFactory
    {
        internal Regression ConstructRegression(RegressionData regressionData, RegressionType regressionType = RegressionType.Linear, WeightingFactor weightingFactor = WeightingFactor.OneOverXSquared)
        {
            switch (regressionType)
            {
                case RegressionType.Linear:
                    return new LinearRegression(regressionData);
                    break;
                case RegressionType.Quadratic:
                    throw new NotImplementedException();
                    break;
                default:
                    throw new Exception("invalid regressionType");
                    break;
            }
        }
    }
}
