using LIMS.Data;
using LIMS.Enums;
using System;
using System.Collections.Generic;

namespace LIMS.Model.Regression
{
    public class LinearRegression
    {
        public LinearRegression(AnalystExport analystExport)
        {
            CreateRegressionDataPoints(analystExport);
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
