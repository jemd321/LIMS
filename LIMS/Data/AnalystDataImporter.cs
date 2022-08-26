using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LIMS.Enums;
using LIMS.Model;
using LIMS.Model.RegressionModels;

namespace LIMS.Data
{
    /// <summary>
    /// Parses the string output of exported text files from Analyst LC-MS/MS data processing software into regression data.
    /// </summary>
    public class AnalystDataImporter : IDataImporter
    {
        /// <summary>
        /// Parses the raw string data that has been read from an Analyst export .txt file into regression data.
        /// </summary>
        /// <param name="rawAnalystData">The entire contents of an Analyst export .txt file as a string.</param>
        /// <returns>The parsed data, as <c>RegressionData</c> format which can be used in a regression.</returns>
        public RegressionData ParseImportedRawData(string rawAnalystData)
        {
            // Each sample must be categorised as one of the following three types:
            List<Standard> standards = new();
            List<QualityControl> qualityControls = new();
            List<Unknown> unknowns = new();

            AnalystExport analystExport = ParseAnalystExport(ref rawAnalystData);

            foreach (var dataRow in analystExport.DataRows)
            {
                SampleType sampleType = dataRow.SampleType;
                switch (sampleType)
                {
                    case SampleType.Standard:
                        standards.Add(new Standard
                        {
                            NominalConcentration = dataRow.NominalConcentration,
                            InstrumentResponse = dataRow.Area,
                            SampleName = dataRow.SampleName,
                        });
                        break;
                    case SampleType.QualityControl:
                        qualityControls.Add(new QualityControl
                        {
                            NominalConcentration = dataRow.NominalConcentration,
                            InstrumentResponse = dataRow.Area,
                            SampleName = dataRow.SampleName,
                        });
                        break;
                    case SampleType.Unknown:
                        unknowns.Add(new Unknown
                        {
                            // Unknown samples have no nominal concentration, unlike Standards or QC samples.
                            InstrumentResponse = dataRow.Area,
                            SampleName = dataRow.SampleName,
                        });
                        break;
                    default:
                        break;
                }
            }

            return new RegressionData
            {
                Standards = standards,
                QualityControls = qualityControls,
                Unknowns = unknowns,
            };
        }

        // TODO refactor to private by updating tests
        public static AnalystExport ParseAnalystExport(ref string analystExport)
        {
            // Analyst files are split into a header and data rows. The header has two subsections, peak info and regression info.
            // Regression information is calculated by Analyst separately and so can be discarded.
            var headerPeakInfo = new List<AnalystExportHeaderPeakInfo>();
            var headerRegressionInfo = new List<AnalystExportHeaderRegressionInfo>();
            var dataRows = new List<AnalystExportRow>();

            using var reader = new StringReader(analystExport);

            // Token to keep track of which section of the export we are in.
            var currentSection = AnalystExportSections.Header;
            var headerBuffer = new List<string>();

            int dataRowHeaderSeparatorCount = 0;
            const int ENDTOKEN = -1;
            while (reader.Peek() != ENDTOKEN)
            {
                string line = reader.ReadLine();

                // An empty line is used as a separator to tell the header sections apart.
                if (string.IsNullOrEmpty(line))
                {
                    switch (currentSection)
                    {
                        case AnalystExportSections.Header:
                            if (BlockIsRegressionInfo(headerBuffer))
                            {
                                // Process regression info header block and move on to data rows.
                                headerRegressionInfo.Add(ProcessRegressionInfo(headerBuffer));
                                currentSection = AnalystExportSections.DataRows;
                                dataRowHeaderSeparatorCount++;
                                headerBuffer.Clear();
                            }
                            else
                            {
                                // Still further peak information to process in the header section.
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
                    // Three empty lines indicates the start of the data row section, of which the first line is the column headers.
                    if (dataRowHeaderSeparatorCount == 3)
                    {
                        VerifyDataRowHeaders(line);
                        dataRowHeaderSeparatorCount = 0;
                        continue;
                    }

                    dataRows.Add(ProcessDataRow(line));
                    continue;
                }

                // If we are not in the data row section or on an empty line we should collect the line as header information.
                headerBuffer.Add(line);
            }

            return new AnalystExport()
            {
                Peaks = headerPeakInfo,
                RegressionInfo = headerRegressionInfo[0],
                DataRows = dataRows,
            };
        }

        private static bool BlockIsRegressionInfo(List<string> buffer)
        {
            // 'F' is always the first letter of this section.
            return buffer[0].StartsWith('F');
        }

        private static AnalystExportHeaderPeakInfo ProcessPeakInfo(List<string> headerBuffer)
        {
            // Tabs are used on some rows to pack information in one line, so splitting is required.
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
                TransitionMRM = transitionMRM,
            };
        }

        private static AnalystExportHeaderRegressionInfo ProcessRegressionInfo(List<string> headerBuffer)
        {
            // Tabs are used on some rows to pack information in one line, so splitting is required.
            string[] firstLine = headerBuffer[0].Split('\t');
            RegressionType regression = ParseRegression(firstLine[1]);
            WeightingFactor weightingFactor = ParseWeightingFactor(firstLine[3]);

            double? a = double.Parse(headerBuffer[1].Split('\t')[1]);
            double? b = double.Parse(headerBuffer[2].Split('\t')[1]);
            double? c = double.Parse(headerBuffer[3].Split('\t')[1]);
            double rSquared = double.Parse(headerBuffer[4].Split('\t')[1]);

            return new AnalystExportHeaderRegressionInfo()
            {
                Regression = regression,
                WeightingFactor = weightingFactor,
                A = a,
                B = b,
                C = c,
                RSquared = rSquared,
            };
        }

        private static AnalystExportRow ProcessDataRow(string line)
        {
            string[] data = line.Split('\t');
            return new AnalystExportRow()
            {
                SampleName = data[0],
                SampleID = int.Parse(data[1]),
                SampleType = ParseSampleType(data[2]),
                SampleDescription = data[3] == "none" ? string.Empty : data[3],
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
                AnalyteAnnotation = data[22] == "N/A" ? string.Empty : data[22],
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
                StandardQueryStatus = data[35] == "N/A" ? string.Empty : data[35],
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

        private static TransitionMRM ParseTransition(string transition)
        {
            var splitTransition = transition.Split('/');
            double q1 = double.Parse(splitTransition[0]);
            double q3 = double.Parse(splitTransition[1]);
            return new TransitionMRM() { Q1 = q1, Q3 = q3 };
        }

        private static RegressionType ParseRegression(string inputRegression)
        {
            return inputRegression switch
            {
                "Linear" => RegressionType.Linear,
                "Quadratic" => RegressionType.Quadratic,
                _ => throw new FileFormatException("unrecognised regression type in analyst result table export"),
            };
        }

        private static WeightingFactor ParseWeightingFactor(string inputWeightingFactor)
        {
            return inputWeightingFactor switch
            {
                "1  / x" => WeightingFactor.OneOverX,
                "1  / (x * x)" => WeightingFactor.OneOverXSquared,
                "1  / y" => WeightingFactor.OneOverY,
                "1  / (y * y)" => WeightingFactor.OneOverYSquared,
                _ => throw new FileFormatException("unrecognised weighting factor in analyst result table export"),
            };
        }

        private static SampleType ParseSampleType(string sampleType)
        {
            return sampleType switch
            {
                "Unknown" => SampleType.Unknown,
                "Standard" => SampleType.Standard,
                "Quality Control" => SampleType.QualityControl,
                _ => throw new FileFormatException("Invalid sample type (STD/QC etc.) in analyst export"),
            };
        }

        private static Units ParseUnits(string units)
        {
            return units switch
            {
                "ng/mL" => Units.ng_mL,
                _ => throw new FileFormatException("Unrecognised units in analyst export"),
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
                string.Empty,
            };
            if (headers.SequenceEqual(validHeaderFormat))
            {
                throw new FileFormatException("Data Row headers in export do not match expected format");
            }
        }
    }
}
