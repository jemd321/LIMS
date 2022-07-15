using LIMS.Enums;
using System;
using System.Collections.Generic;

namespace LIMS.Model.RegressionModels
{
    public class LinearRegression : Regression
    {
        public LinearRegression(RegressionData regressionData, WeightingFactor weightingFactor)
        {
            RegressionData = regressionData;
            UpdateRegression();
        }

        public RegressionData RegressionData { get; private set; }
        public WeightingFactor WeightingFactor { get; private set; }
        public double? Gradient { get; private set; }
        public double? YIntercept { get; private set; }

        public void UpdateRegression()
        {
            switch (WeightingFactor)
            {
                case WeightingFactor.Unweighted:
                    PerformUnweightedRegression();
                    break;
                case WeightingFactor.OneOverXHalf | WeightingFactor.OneOverX | WeightingFactor.OneOverXSquared:
                    PerformXWeightedRegression();
                    break;
                case WeightingFactor.OneOverYHalf | WeightingFactor.OneOverY | WeightingFactor.OneOverYSquared:
                    PerformYWeightedRegression();
                    break;
                default:
                    throw new ArgumentException("Invalid Weighting factor supplied to regression");
            }

            UpdateAllCalculatedConcentrations();
            CalculateStandardAndQCAccuracy();
            CalculateQCPresicion();
        }

        private void PerformUnweightedRegression()
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
            CalculateUnweightedGradient(xSum, ySum, xySum, xSquaredSum, activeStandards);
            CalculateUnweightedYIntercept(xSum, ySum, activeStandards);
        }

        private void PerformXWeightedRegression()
        {
            Func<double?, double?> wFactorEquation;
            switch (WeightingFactor)
            {
                case WeightingFactor.OneOverXHalf:
                    wFactorEquation = x => Math.Sqrt((double)x);
                    break;
                case WeightingFactor.OneOverX:
                    wFactorEquation = x => x;
                    break;
                case WeightingFactor.OneOverXSquared:
                    wFactorEquation = x => x * x;
                    break;
                default:
                    throw new ArgumentException("Invalid Weighting factor during gradient calculation");
            }

            double? wxSum = 0;
            double? wySum = 0;
            double? wxySum = 0;
            double? wxSquaredSum = 0;
            double? w = 0;
            double? wSum = 0;
            foreach (var standard in RegressionData.Standards)
            {
                if (!standard.IsActive)
                {
                    continue;
                }
                double? x = standard.NominalConcentration;
                double? y = standard.InstrumentResponse;
                w = wFactorEquation(x);
                wxSum += (w * x);
                wySum += (w * y);
                wxySum += (w * x * y);
                wxSquaredSum += (w * (x * x));
                wSum += w;
            }
            CalculateWeightedGradient(wxSum, wySum, wxySum, wxSquaredSum, wSum);
            CalculateWeightedYIntercept(wxSum, wySum, wSum);
        }
        private void PerformYWeightedRegression()
        {
            Func<double?, double?> wFactorEquation;
            switch (WeightingFactor)
            {
                case WeightingFactor.OneOverYHalf:
                    wFactorEquation = y => Math.Sqrt((double)y);
                    break;
                case WeightingFactor.OneOverY:
                    wFactorEquation = y => y;
                    break;
                case WeightingFactor.OneOverYSquared:
                    wFactorEquation = y => y * y;
                    break;
                default:
                    throw new ArgumentException("Invalid Weighting factor during gradient calculation");
            }

            double? wxSum = 0;
            double? wySum = 0;
            double? wxySum = 0;
            double? wxSquaredSum = 0;
            double? w = 0;
            double? wSum = 0;
            foreach (var standard in RegressionData.Standards)
            {
                if (!standard.IsActive)
                {
                    continue;
                }
                double? x = standard.NominalConcentration;
                double? y = standard.InstrumentResponse;
                w = wFactorEquation(y);
                wxSum += (w * x);
                wySum += (w * y);
                wxySum += (w * x * y);
                wxSquaredSum += (w * (x * x));
                wSum += w;
            }
            CalculateWeightedGradient(wxSum, wySum, wxySum, wxSquaredSum, wSum);
            CalculateWeightedYIntercept(wxSum, wySum, wSum);
        }

        private void CalculateUnweightedGradient(double? xSum, double? ySum, double? xySum, double? xSquaredSum, int activeStandards)
        {
            Gradient =
                ((activeStandards * xySum) - (xSum * ySum)) /
                ((activeStandards * xSquaredSum) - (xSum * xSum));
        }

        private void CalculateUnweightedYIntercept(double? xSum, double? ySum, int activeStandards)
        {
            YIntercept =
                (ySum - (Gradient * xSum)) / activeStandards;
        }

        private void CalculateWeightedGradient(double? wxSum, double? wySum, double? wxySum, double? wxSquaredSum, double? wSum)
        {

            Gradient =
                ((wSum * wxySum) - (wxSum * wySum)) /
                ((wSum * wxSquaredSum) - (wxSum * wxSum));
        }

        private void CalculateWeightedYIntercept(double? wxSum, double? wySum, double? wSum)
        {
            YIntercept =
                (wySum - (Gradient * wxSum)) / wSum;
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

        private void CalculateStandardAndQCAccuracy()
        {
            foreach (var standard in RegressionData.Standards)
            {
                standard.Accuracy = CalculateAccuracy(standard.CalculatedConcentration, standard.NominalConcentration);
            }
            foreach (var qualityControl in RegressionData.QualityControls)
            {
                qualityControl.Accuracy = CalculateAccuracy(qualityControl.CalculatedConcentration, qualityControl.NominalConcentration);
            }
        }

        private double? CalculateAccuracy(double? observedConcentration, double? nominalConcentration)
        {
            return nominalConcentration == 0 ? null : ((observedConcentration - nominalConcentration) / nominalConcentration) * 100;
        }

        private void CalculateQCPresicion()
        {
            // TODO
        }




    }


}
