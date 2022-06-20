using LIMS.Enums;
using LIMS.Model;
using LIMS.Model.RegressionModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LIMS.Data
{
    public class AnalystDataProvider : IRegressionDataProvider
    {
        public async Task<RegressionData> GetRegressionData(string rawAnalystData)
        {
            List<Standard> standards = new();
            List<QualityControl> qualityControls = new();
            List<Unknown> unknowns = new();

            await Task.Run(() =>
            {
                AnalystExport analystExport = ParseAnalystExport(ref rawAnalystData);

                foreach (var dataRow in analystExport.DataRows)
                {
                    SampleType sampleType = dataRow.SampleType;
                    switch (sampleType)
                    {
                        case SampleType.Standard:
                            standards.Add(new Standard
                            {
                                X = dataRow.NominalConcentration,
                                Y = dataRow.Area,
                                SampleName = dataRow.SampleName,
                            });
                            break;
                        case SampleType.QualityControl:
                            qualityControls.Add(new QualityControl
                            {
                                X = dataRow.NominalConcentration,
                                Y = dataRow.Area,
                                SampleName = dataRow.SampleName,
                            });
                            break;
                        case SampleType.Unknown:
                            unknowns.Add(new Unknown
                            {
                                Y = dataRow.Area,
                                SampleName = dataRow.SampleName,
                            });
                            break;
                        default:
                            break;
                    }
                }
            });
            return new RegressionData
            {
                Standards = standards,
                QualityControls = qualityControls,
                Unknowns = unknowns
            };
        }

        private AnalystExport ParseAnalystExport(ref string analystExport)
        {
            var headerPeakInfo = new List<AnalystExportHeaderPeakInfo>();
            var headerRegressionInfo = new List<AnalystExportHeaderRegressionInfo>();
            var dataRows = new List<AnalystExportRow>();

            using var reader = new StringReader(analystExport);

            var currentSection = AnalystExportSections.Header;
            var headerBuffer = new List<string>();

            int dataRowHeaderSeparatorCount = 0;
            const int ENDTOKEN = -1;
            while (reader.Peek() != ENDTOKEN)
            {
                string line = reader.ReadLine();
                if (string.IsNullOrEmpty(line))
                {
                    switch (currentSection)
                    {
                        case AnalystExportSections.Header:
                            if (BlockIsRegressionInfo(headerBuffer))
                            {
                                headerRegressionInfo.Add(ProcessRegressionInfo(headerBuffer));
                                currentSection = AnalystExportSections.DataRows;
                                dataRowHeaderSeparatorCount++;
                                headerBuffer.Clear();
                            }
                            else
                            {
                                headerPeakInfo.Add(ProcessPeakInfo(headerBuffer));
                                headerBuffer.Clear();
                            }
                            continue;
                        case AnalystExportSections.DataRows:
                            dataRowHeaderSeparatorCount++;
                            continue;
                    }
                }
                if (currentSection == AnalystExportSections.DataRows)
                {
                    if (dataRowHeaderSeparatorCount == 3)
                    {
                        VerifyDataRowHeaders(line);
                        dataRowHeaderSeparatorCount = 0;
                        continue;
                    }
                    dataRows.Add(ProcessDataRow(line));
                    continue;
                }
                headerBuffer.Add(line);
            }
            return new AnalystExport()
            {
                Peaks = headerPeakInfo,
                RegressionInfo = headerRegressionInfo[0],
                DataRows = dataRows
            };
        }

        private bool BlockIsRegressionInfo(List<string> buffer)
        {
            return buffer[0].StartsWith('F');
        }

        private AnalystExportHeaderPeakInfo ProcessPeakInfo(List<string> headerBuffer)
        {
            string peakName = headerBuffer[0].Split(' ')[2];
            bool isInternalStandard = headerBuffer[1].TrimEnd() == "Use as Internal Standard";
            string internalStandard = null;
            if (!isInternalStandard)
            {
                internalStandard = headerBuffer[1].Split(' ')[2];
            }
            var transitionMRM = ParseTransition(headerBuffer[2].Split(' ')[2]);
            return new AnalystExportHeaderPeakInfo()
            {
                PeakName = peakName,
                IsInternalStandard = isInternalStandard,
                InternalStandard = internalStandard,
                TransitionMRM = transitionMRM
            };
        }

        private AnalystExportHeaderRegressionInfo ProcessRegressionInfo(List<string> headerBuffer)
        {
            string[] firstLine = headerBuffer[0].Split('\t');
            RegressionType regression = ParseRegression(firstLine[1]);
            WeightingFactor weightingFactor = ParseWeightingFactor(firstLine[3]);

            double? a, b, c = null;
            a = double.Parse(headerBuffer[1].Split('\t')[1]);
            b = double.Parse(headerBuffer[2].Split('\t')[1]);
            c = double.Parse(headerBuffer[3].Split('\t')[1]);

            double rSquared = double.Parse(headerBuffer[4].Split('\t')[1]);
            return new AnalystExportHeaderRegressionInfo()
            {
                Regression = regression,
                WeightingFactor = weightingFactor,
                A = a,
                B = b,
                C = c,
                RSquared = rSquared
            };
        }

        private AnalystExportRow ProcessDataRow(string line)
        {
            string[] data = line.Split('\t');
            return new AnalystExportRow()
            {
                SampleName = data[0],
                SampleID = int.Parse(data[1]),
                SampleType = ParseSampleType(data[2]),
                SampleDescription = data[3] == "none" ? "" : data[3],
                SetNumber = int.Parse(data[4]),
                AcquisitonMethod = data[5],
                AcquisitionDate = DateTime.Parse(data[6]),
                RackType = data[7],
                RackNumber = int.Parse(data[8]),
                VialPosition = int.Parse(data[9]),
                PlateType = data[10],
                PlateNumber = int.Parse(data[11]),
                FileName = data[12],
                DilutionFactor = double.Parse(data[13]),
                WeightToVolumeRatio = double.Parse(data[14]),
                SampleAnnotation1 = data[15],
                SampleAnnotation2 = data[16],
                PeakName = data[17],
                Units = ParseUnits(data[18]),
                Area = double.Parse(data[19]),
                Height = double.Parse(data[21]),
                AnalyteAnnotation = data[22] == "N/A" ? "" : data[22],
                NominalConcentration = data[23] == "N/A" ? 0d : double.Parse(data[23]),
                RetentionTime = double.Parse(data[24]),
                ExpectedRetentionTime = double.Parse(data[25]),
                RetentionTimeWindow = double.Parse(data[26]),
                CentroidLocation = double.Parse(data[27]),
                StartScan = int.Parse(data[28]),
                StartTime = double.Parse(data[29]),
                EndScan = int.Parse(data[30]),
                EndTime = double.Parse(data[31]),
                IntegrationType = data[32],
                SignalToNoiseRatio = data[33] == "N/A" ? null : double.Parse(data[33]),
                PeakWidth = double.Parse(data[34]),
                StandardQueryStatus = data[35] == "N/A" ? "" : data[35],
                AnalyteTransitionMRM = ParseTransition(data[36].Split(' ')[0]),
                AnalyteToISAreaRatio = data[38] == "#DIV/0!" ? null : double.Parse(data[38]),
                AnalyteToISHeightRatio = data[39] == "#DIV/0!" ? null : double.Parse(data[39]),
                AnalytePeakWidthAtHalfHeight = double.Parse(data[42]),
                AnalyteSlopeOfBaseline = data[43] == "#DIV/0!" ? 0d : double.Parse(data[43]),
                AnalyteProcessingAlgorithm = data[44],
                AnalytePeakAsymmetry = double.Parse(data[45]),
                ISPeakName = data[46],
                ISUnits = ParseUnits(data[47]),
                ISArea = double.Parse(data[48]),
                ISHeight = double.Parse(data[50]),
                ISConcentration = double.Parse(data[52]),
                ISRetentionTime = double.Parse(data[53]),
                ISExpectedRetentionTime = double.Parse(data[54]),
                ISRetentionTimeWindow = double.Parse(data[55]),
                ISCentroidLocation = double.Parse(data[56]),
                ISStartScan = int.Parse(data[57]),
                ISStartTime = double.Parse(data[58]),
                ISStopScan = int.Parse(data[59]),
                ISEndTime = double.Parse(data[60]),
                ISIntegrationType = data[61],
                ISSignalToNoiseRatio = data[62] == "N/A" ? 0d : double.Parse(data[62]),
                ISPeakWidth = double.Parse(data[63]),
                ISTransitionMRM = ParseTransition(data[64].Split(' ')[0]),
                ISPeakWidthAtHalfHeight = double.Parse(data[67]),
                ISSlopeOfBaseline = double.Parse(data[68]),
                ISProcessingAlgorithm = data[69],
                ISPeakAsymmetry = double.Parse(data[70]),
                UseRecord = data[71] == "1",
                RecordModified = data[72] == "1",
                CalculatedConcentration = data[73] == "#DIV/0!" || data[73] == "No Peak" ? 0d : double.Parse(data[73]),
                RelativeRetentionTime = double.Parse(data[75]),
                Accuracy = data[76] == "N/A" ? null : double.Parse(data[76]),
                ResponseFactor1 = data[77] == "N/A" ? null : double.Parse(data[77]),
                ResponseFactor2 = data[78] == "N/A" ? null : double.Parse(data[78]),
                ResponseFactor3 = data[79] == "N/A" ? null : double.Parse(data[79]),
            };
        }

        private TransitionMRM ParseTransition(string transition)
        {
            var splitTransition = transition.Split('/');
            double Q1 = double.Parse(splitTransition[0]);
            double Q3 = double.Parse(splitTransition[1]);
            return new TransitionMRM() { Q1 = Q1, Q3 = Q3 };
        }

        private RegressionType ParseRegression(string inputRegression)
        {
            RegressionType regression;
            switch (inputRegression)
            {
                case "Linear":
                    regression = RegressionType.Linear;
                    break;
                case "Quadratic":
                    regression = RegressionType.Quadratic;
                    break;
                default:
                    throw new FileFormatException("unrecognised regression type in analyst result table export");
            }
            return regression;
        }

        private WeightingFactor ParseWeightingFactor(string inputWeightingFactor)
        {
            WeightingFactor weightingFactor;
            switch (inputWeightingFactor)
            {
                case "1":
                    weightingFactor = WeightingFactor.One;
                    break;
                case "1  / x":
                    weightingFactor = WeightingFactor.OneOverX;
                    break;
                case "1  / (x * x)":
                    weightingFactor = WeightingFactor.OneOverXSquared;
                    break;
                default:
                    throw new FileFormatException("unrecognised weighting factor in analyst result table export");
            }
            return weightingFactor;
        }

        private SampleType ParseSampleType(string sampleType)
        {
            switch (sampleType)
            {
                case "Unknown":
                    return SampleType.Unknown;
                case "Standard":
                    return SampleType.Standard;
                case "Quality Control":
                    return SampleType.QualityControl;
                default:
                    throw new FileFormatException("Invalid sample type (STD/QC etc.) in analyst export");
            }
        }

        private Units ParseUnits(string units)
        {
            switch (units)
            {
                case "ng/mL":
                    return Units.ng_mL;
                default:
                    throw new FileFormatException("Unrecognised units in analyst export");
            }
        }

        private void VerifyDataRowHeaders(string headerRow)
        {
            string[] headers = headerRow.Split('\t');
            string[] validHeaderFormat =
            {
                "Sample Name",
                "Sample ID",
                "Filetype",
                "Sample Desc.",
                "Set Number",
                "Acquisition Method",
                "Date of Acq.",
                "Rack Type",
                "Rack Number",
                "Vial Position",
                "Plate Type",
                "Plate Number",
                "Filename",
                "Dil.Factor",
                "Weight To Volume Ratio",
                "Sample Annotation",
                "Sample Annotation",
                "Peak Name",
                "Units",
                "Area",
                "Analyte Peak Height for DAD",
                "Height",
                "Analyte Annotation",
                "Conc.",
                "R.T.",
                "Expected RT",
                "RT Window",
                "Centroid Location",
                "Start Scan",
                "Start Time",
                "End Scan",
                "End Time",
                "Integ.Type",
                "S/N",
                "Peak Width",
                "Standard Query Status",
                "Analyte Mass Range",
                "Analyte Peak Area for DAD",
                "Analyte/IS Area",
                "Analyte/IS Height",
                "Analyte Wavelength Ranges",
                "Analyte UV Range",
                "Analyte Peak Width at 50% Height",
                "Analyte Slope of Baseline",
                "Analyte Processing Alg.",
                "Analyte Peak Asymmetry",
                "IS Peak Name",
                "IS Units",
                "IS Area",
                "IS Peak Area for DAD",
                "IS Height",
                "IS Peak Height for DAD",
                "IS Conc.",
                "IS R.T.",
                "IS Expected RT",
                "IS RT Window",
                "IS Centroid Location",
                "IS Start Scan",
                "IS Start Time",
                "IS Stop Scan",
                "IS End Time",
                "IS Integ.Type",
                "IS S/N",
                "IS Peak Width",
                "IS Mass Range",
                "IS Wavelength Ranges",
                "IS UV Range",
                "IS Peak Width at 50% Height",
                "IS Slope of Baseline",
                "IS Processing Alg.",
                "IS Peak Asymmetry",
                "Use Record",
                "Record Modified",
                "Calc.Conc.",
                "Calculated Concentration for DAD",
                "Rel.R.T.",
                "Accuracy",
                "Resp.Factor",
                "Resp.Factor",
                "Resp.Factor",
                ""
            };
            if (headers.SequenceEqual(validHeaderFormat))
            {
                throw new FileFormatException("Data Row headers in export do not match expected format");
            }
        }
    }
}
