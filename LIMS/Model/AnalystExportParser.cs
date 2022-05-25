using LIMS.Data;
using LIMS.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LIMS.Model
{
    public static class AnalystExportParser
    {
        public static AnalystExport ParseAnalystExport(string analystExport)
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
                AnalystRegressionInfo = headerRegressionInfo[0],
                DataRows = dataRows
            };
        }

        private static AnalystExportHeaderPeakInfo ProcessPeakInfo(List<string> headerBuffer)
        {
            string peakName = headerBuffer[0].Split(' ')[2];
            bool isInternalStandard = headerBuffer[1].TrimEnd() == "Use as Internal Standard";
            string internalStandard = null;
            if (!isInternalStandard)
            {
                internalStandard = headerBuffer[1].Split(' ')[2];
            }
            var transitionMRM = ParseTransition(headerBuffer[2]);
            return new AnalystExportHeaderPeakInfo()
            {
                PeakName = peakName,
                IsInternalStandard = isInternalStandard,
                InternalStandard = internalStandard,
                TransitionMRM = transitionMRM
            };
        }

        private static TransitionMRM ParseTransition(string line)
        {
            string transition = line.Split(' ')[2];
            var splitTransition = transition.Split('/');
            double Q1 = double.Parse(splitTransition[0]);
            double Q3 = double.Parse(splitTransition[1]);
            return new TransitionMRM() { Q1 = Q1, Q3 = Q3 };
        }

        private static AnalystExportHeaderRegressionInfo ProcessRegressionInfo(List<string> headerBuffer)
        {
            string[] firstLine = headerBuffer[0].Split('\t');
            Regression regression = ParseRegression(firstLine[1]);
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
        private static Regression ParseRegression(string inputRegression)
        {
            Regression regression;
            switch (inputRegression)
            {
                case "Linear":
                    regression = Regression.Linear;
                    break;
                case "Quadratic":
                    regression = Regression.Quadratic;
                    break;
                default:
                    throw new ArgumentException("unrecognised regression type in analyst result table export");
            }
            return regression;
        }

        private static WeightingFactor ParseWeightingFactor(string inputWeightingFactor)
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

        private static bool BlockIsRegressionInfo(List<string> buffer)
        {
            return buffer[0].StartsWith('F');
        }


        private static AnalystExportRow ProcessDataRow(string line)
        {
            string[] data = line.Split('\t');
            return new AnalystExportRow()
            {
                SampleName = data[0],
                SampleID = data[1],
                FileType = data[2],
                SampleDescription = data[3],
                SetNumber = data[4],
                AcquisitonMethod = data[5],
                AcquisitionDate = data[6],
                RackType = data[7],
                RackNumber = data[8],
                VialPosition = data[9],
                PlateType = data[10],
                PlateNumber = data[11],
                FileName = data[12],
                DilutionFactor = data[13],
                WeightToVolumeRatio = data[14],
                SampleAnnotation1 = data[15],
                SampleAnnotation2 = data[16],
                PeakName = data[17],
                Units = data[18],
                Area = data[19],
                AnalytePeakHeightForDAD = data[20],
                Height = data[21],
                AnalyteAnnotation = data[22],
                Concentration = data[23],
                RetentionTime = data[24],
                ExpectedRetentionTime = data[25],
                RetentionTimeWindow = data[26],
                CentroidLocation = data[27],
                StartScan = data[28],
                StartTime = data[29],
                EndScan = data[30],
                EndTime = data[31],
                IntegrationType = data[32],
                SignalToNoiseRatio = data[33],
                PeakWidth = data[34],
                StandardQueryStatus = data[35],
                AnalyteMassRange = data[36],
                AnalytePeakAreaForDAD = data[37],
                AnalyteToISAreaRatio = data[38],
                AnalyteToISHeightRatio = data[39],
                AnalyteWavelengthRanges = data[40],
                AnalyteUVRange = data[41],
                AnalytePeakWidthAtHalfHeight = data[42],
                AnalyteSlopeOfBaseline = data[43],
                AnalyteProcessingAlgorithm = data[44],
                AnalytePeakAsymmetry = data[45],
                ISPeakName = data[46],
                ISUnits = data[47],
                ISArea = data[48],
                ISPeakAreaForDAD = data[49],
                ISHeight = data[50],
                ISConcentration = data[51],
                ISRetentionTime = data[52],
                ISExpectedRetentionTime = data[53],
                ISRetentionTimeWindow = data[54],
                ISCentroidLocation = data[55],
                ISStartScan = data[56],
                ISStartTime = data[57],
                ISStopScan = data[58],
                ISEndTime = data[59],
                ISIntegrationType = data[60],
                ISSignalToNoiseRatio = data[61],
                ISPeakWidth = data[62],
                ISMassRange = data[63],
                ISWaveLengthRanges = data[64],
                ISUVRange = data[65],
                ISPeakWidthAtHalfHeight = data[66],
                ISSlopeOfBaseline = data[67],
                ISProcessingAlgorithm = data[68],
                ISPeakAsymmetry = data[69],
                UseRecord = data[70],
                RecordModified = data[71],
                CalculatedConcentrationForDAD = data[72],
                RelativeRetentionTime = data[73],
                Accuracy = data[74],
                ResponseFactor1 = data[75],
                ResponseFactor2 = data[76],
                ResponseFactor3 = data[77]
            };
        }
        private static void VerifyDataRowHeaders(string headerRow)
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
