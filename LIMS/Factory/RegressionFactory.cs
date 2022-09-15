using System;
using LIMS.Enums;
using LIMS.Model;
using LIMS.Model.RegressionModels;

namespace LIMS.Factory
{
    /// <summary>
    /// A factory class that constructs a regression given the desired type of regression.
    /// </summary>
    public class RegressionFactory : IRegressionFactory
    {
        /// <summary>
        /// Constructs a regression, given data and the desired regression type and weighing factor.
        /// </summary>
        /// <param name="regressionData">A <see cref="RegressionData"/> object that contains the data which will be part of the regression.</param>
        /// <param name="regressionType">The <see cref="RegressionType"/> type of regression to construct.</param>
        /// <param name="weightingFactor">The <see cref="WeightingFactor"/> that should be applied to the regression.</param>
        /// <returns>A <see cref="Regression"/> object which allows the calculation of the regression.</returns>
        public Regression ConstructRegression(RegressionData regressionData, RegressionType regressionType = RegressionType.Linear, WeightingFactor weightingFactor = WeightingFactor.OneOverXSquared)
        {
            return regressionType switch
            {
                RegressionType.Linear => new LinearRegression(regressionData, weightingFactor),
                RegressionType.Quadratic => throw new NotImplementedException(),
                _ => throw new Exception("invalid regressionType"),
            };
        }
    }
}
