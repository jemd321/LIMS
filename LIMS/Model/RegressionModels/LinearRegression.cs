using LIMS.Enums;
using System;
using System.Collections.Generic;

namespace LIMS.Model.RegressionModels
{
    public class LinearRegression : Regression
    {
        public LinearRegression(AnalystExport analystExport)
        {
            CreateRegressionDataPoints(analystExport);
            UpdateRegression();
        }

        public void UpdateRegression()
        {
            double? xSum = 0;
            double? ySum = 0;
            double? xySum = 0;
            double? xSquaredSum = 0;
            int activeStandards = 0;

            foreach (var standard in Standards)
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

        public List<Standard> Standards { get; init; } = new();
        public List<QualityControl> QualityControls { get; init; } = new();
        public List<Unknown> Unknowns { get; init; } = new();

        private void CreateRegressionDataPoints(AnalystExport analystExport)
        {
            foreach (var dataRow in analystExport.DataRows)
            {
                ProcessDataRow(dataRow);
            }
        }

        private void ProcessDataRow(AnalystExportRow dataRow)
        {
            SampleType sampleType = dataRow.SampleType;
            switch (sampleType)
            {
                case SampleType.Standard:
                    Standards.Add(new Standard
                    {
                        X = dataRow.NominalConcentration,
                        Y = dataRow.Area,
                        SampleName = dataRow.SampleName,
                    });
                    break;
                case SampleType.QualityControl:
                    QualityControls.Add(new QualityControl
                    {
                        X = dataRow.NominalConcentration,
                        Y = dataRow.Area,
                        SampleName = dataRow.SampleName,
                    });
                    break;
                case SampleType.Unknown:
                    Unknowns.Add(new Unknown
                    {
                        Y = dataRow.Area,
                        SampleName = dataRow.SampleName,
                    });
                    break;
                default:
                    break;
            }
        }
    }
}
