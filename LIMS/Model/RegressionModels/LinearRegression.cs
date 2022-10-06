using System;
using LIMS.Enums;

namespace LIMS.Model.RegressionModels
{
    /// <summary>
    /// A class representing a linear regression, allowing the calculation of the line equation, standard & QC accuracy and unknown concentration. Inherits from <see cref="Regression"/>.
    /// </summary>
    public class LinearRegression : Regression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LinearRegression"/> class.
        /// </summary>
        /// <param name="regressionData">The <see cref="RegressionData"/> that will be used to perform the regression.</param>
        /// <param name="weightingFactor">The weighting factor to be applied to the regression. By default this is 1/x^2 since this is the most common.</param>
        public LinearRegression(RegressionData regressionData, WeightingFactor weightingFactor)
        {
            RegressionData = regressionData;
            WeightingFactor = weightingFactor;
            UpdateRegression();
        }

        /// <summary>
        /// Gets the type of the regression.
        /// </summary>
        public static new RegressionType RegressionType => RegressionType.Linear;

        /// <summary>
        /// Re-calculates all parameters of the regression. If the regressionData is updated this method should be called to recalculate.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when an invalid weighting factor is supplied to the regression.</exception>
        public override void UpdateRegression()
        {
            switch (WeightingFactor)
            {
                case WeightingFactor.Unweighted:
                    PerformUnweightedRegression();
                    break;
                case WeightingFactor.OneOverXHalf:
                case WeightingFactor.OneOverX:
                case WeightingFactor.OneOverXSquared:
                    PerformXWeightedRegression();
                    break;
                case WeightingFactor.OneOverYHalf:
                case WeightingFactor.OneOverY:
                case WeightingFactor.OneOverYSquared:
                    PerformYWeightedRegression();
                    break;
                default:
                    throw new ArgumentException("Invalid Weighting factor supplied to regression");
            }

            UpdateAllCalculatedConcentrations();
            CalculateStandardAndQCAccuracy();
        }

#pragma warning disable SA1119 // Statement should not use unnecessary parenthesis - suppressed since paranthesis make the maths clearer here.
        private static double? CalculateAccuracy(double? observedConcentration, double? nominalConcentration)
        {
            // expressed without *100, as string formating as a percantage will be applied at UI level.
            return nominalConcentration == 0 ? null : ((observedConcentration - nominalConcentration) / nominalConcentration);
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
            Func<double?, double?> wFactorEquation = WeightingFactor switch
            {
                WeightingFactor.OneOverXHalf => x => Math.Sqrt((double)x),
                WeightingFactor.OneOverX => x => x,
                WeightingFactor.OneOverXSquared => x => x * x,
                _ => throw new ArgumentException("Invalid Weighting factor during gradient calculation"),
            };
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
            Func<double?, double?> wFactorEquation = WeightingFactor switch
            {
                WeightingFactor.OneOverYHalf => y => Math.Sqrt((double)y),
                WeightingFactor.OneOverY => y => y,
                WeightingFactor.OneOverYSquared => y => y * y,
                _ => throw new ArgumentException("Invalid Weighting factor during gradient calculation"),
            };
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
    }
}
