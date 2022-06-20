using LIMS.Enums;
using System;
using System.Collections.Generic;

namespace LIMS.Model.RegressionModels
{
    public class LinearRegression : Regression
    {
        public LinearRegression(RegressionData regressionData)
        {
            UpdateRegression();
        }

        public RegressionData RegressionData { get; private set; }

        public void UpdateRegression()
        {
            double? xSum = 0;
            double? ySum = 0;
            double? xySum = 0;
            double? xSquaredSum = 0;
            int activeStandards = 0;

            foreach (var standard in RegressionData.Standards)
            {
                if (!standard.IsActive)
                {
                    continue;
                }
                ++activeStandards;
                double? x = standard.X;
                double? y = standard.Y;
                xSum += x;
                ySum += y;
                xySum += x * y;
                xSquaredSum += x * x;
            }

            double? gradient = 
                (activeStandards * xySum) - (xSum * ySum) /
                (activeStandards * xSquaredSum) - (xSum * xSum);
            double? yIntercept =
                ySum - (gradient * xSum) /
                activeStandards;

            CalculateConcentrations();
            CalculateSTDQCBias();
            CalculateQCPresicion();
        }

        private void CalculateQCPresicion()
        {
            throw new NotImplementedException();
        }

        private void CalculateSTDQCBias()
        {
            throw new NotImplementedException();
        }

        private void CalculateConcentrations()
        {
            throw new NotImplementedException();
        }
    }
}
