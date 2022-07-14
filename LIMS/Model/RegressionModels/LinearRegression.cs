using LIMS.Enums;
using System;
using System.Collections.Generic;

namespace LIMS.Model.RegressionModels
{
    public class LinearRegression : Regression
    {
        public LinearRegression(RegressionData regressionData)
        {
            RegressionData = regressionData;
            UpdateRegression();
        }

        public RegressionData RegressionData { get; private set; }

        public double? Gradient { get; private set; }
        public double? YIntercept { get; private set; }
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
                double? x = standard.NominalConcentration;
                double? y = standard.InstrumentResponse;
                xSum += x;
                ySum += y;
                xySum += x * y;
                xSquaredSum += x * x;
            }

            Gradient =
                ((activeStandards * xySum) - (xSum * ySum)) /
                ((activeStandards * xSquaredSum) - (xSum * xSum));
            YIntercept =
                (ySum - (Gradient * xSum)) / activeStandards;

            UpdateAllCalculatedConcentrations();
            CalculateSTDQCBias();
            CalculateQCPresicion();
        }
        private void UpdateAllCalculatedConcentrations()
        {
            foreach (var standard in RegressionData.Standards)
            {
                standard.CalculatedConcentration = CalculateConcentration(standard.InstrumentResponse);
            }
            foreach (var qualityControl in RegressionData.QualityControls)
            {
                qualityControl.CalculatedConcentration = CalculateConcentration(qualityControl.InstrumentResponse);
            }
            foreach (var unknown in RegressionData.Unknowns)
            {
                unknown.CalculatedConcentration = CalculateConcentration(unknown.InstrumentResponse);
            }
        }

        private double? CalculateConcentration(double? instrumentResponse)
        {
            return (instrumentResponse - YIntercept) / Gradient;
        }

        private void CalculateSTDQCBias()
        {
            
        }

        private void CalculateQCPresicion()
        {
            
        }




    }
}
