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
        public ImportedDataParseResult ParseImportedRawData(string rawAnalystData)
        {
            // Each sample must be categorised as one of the following three types:
            List<Standard> standards = new();
            List<QualityControl> qualityControls = new();
            List<Unknown> unknowns = new();

            try
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
                                NominalConcentration = dataRow.NominalConcentration,
                                InstrumentResponse = dataRow.Area,
                                SampleNumber = int.Parse(dataRow.SampleName.Split(' ')[1]),
                                SampleName = dataRow.SampleName.Split(' ')[3] + ' ' + dataRow.SampleName.Split(' ')[4],
                            });
                            break;
                        case SampleType.QualityControl:
                            qualityControls.Add(new QualityControl
                            {
                                NominalConcentration = dataRow.NominalConcentration,
                                InstrumentResponse = dataRow.Area,
                                SampleNumber = int.Parse(dataRow.SampleName.Split(' ')[1]),
                                SampleName = dataRow.SampleName.Split(' ')[3] + ' ' + dataRow.SampleName.Split(' ')[4],
                            });
                            break;
                        case SampleType.Unknown:
                            unknowns.Add(new Unknown
                            {
                                // Unknown samples have no nominal concentration, unlike Standards or QC samples.
                                InstrumentResponse = dataRow.Area,
                                SampleNumber = int.Parse(dataRow.SampleName.Split(' ')[1]),
                                SampleName = dataRow.SampleName.Split(' ')[3] + ' ' + dataRow.SampleName.Split(' ')[4],
                            });
                            break;
                        default:
                            break;
                    }
                }

                var regressionData = new RegressionData
                {
                    Standards = standards,
                    QualityControls = qualityControls,
                    Unknowns = unknowns,
                };

                return new ImportedDataParseResult
                {
                    Data = regressionData,
                    ParseFailureReason = ParseFailureReason.None,
                };
            }
            catch (FileFormatException)
            {
                return new ImportedDataParseResult { ParseFailureReason = ParseFailureReason.InvalidFileFormat };
            }
            catch (ArgumentOutOfRangeException)
            {
                return new ImportedDataParseResult { ParseFailureReason = ParseFailureReason.InvalidFileFormat };
            }
            catch (ArgumentException)
            {
                return new ImportedDataParseResult { ParseFailureReason = ParseFailureReason.InvalidCast };
            }
            catch (SystemException)
            {
                return new ImportedDataParseResult { ParseFailureReason = ParseFailureReason.OtherSystemException };
            }
        }

        private static AnalystExport ParseAnalystExport(ref string analystExport)
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
            var skippedColumns = new List<int>();

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
                        skippedColumns = ProcessDataRowHeaders(line);
                        dataRowHeaderSeparatorCount = 0;
                        continue;
                    }

                    dataRows.Add(ProcessDataRow(line, skippedColumns));
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

            double? c = 0;
            double rSquared;
            if (regression == RegressionType.Quadratic)
            {
                // In a quadratic regression, there are four lines, to the header, with the 3rd being the c term.
                c = double.Parse(headerBuffer[3].Split('\t')[1]);
                rSquared = double.Parse(headerBuffer[4].Split('\t')[1]);
            }
            else
            {
                // The regression is linear, therefore there is no c term to the regression - the 3rd line is the RSquared.
                rSquared = double.Parse(headerBuffer[3].Split('\t')[1]);
            }

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

        private static AnalystExportRow ProcessDataRow(string line, List<int> ignoredColumnIndices)
        {
            List<string> data = line.Split('\t').ToList();

            // Remove columns that should be skipped
            var filteredColumns = data.Where((n, i) => !ignoredColumnIndices.Contains(i)).ToList();
            return new AnalystExportRow()
            {
                SampleName = filteredColumns[0],
                SampleID = int.Parse(filteredColumns[1]),
                SampleType = ParseSampleType(filteredColumns[2]),
                SampleDescription = filteredColumns[3] == "none" ? string.Empty : filteredColumns[3],
                SetNumber = int.Parse(filteredColumns[4]),
                AcquisitonMethod = filteredColumns[5],
                AcquisitionDate = DateTime.Parse(filteredColumns[6]),
                RackType = filteredColumns[7],
                RackNumber = int.Parse(filteredColumns[8]),
                VialPosition = int.Parse(filteredColumns[9]),
                PlateType = filteredColumns[10],
                PlateNumber = int.Parse(filteredColumns[11]),
                FileName = filteredColumns[12],
                DilutionFactor = double.Parse(filteredColumns[13]),
                WeightToVolumeRatio = double.Parse(filteredColumns[14]),
                SampleAnnotation = filteredColumns[15],
                PeakName = filteredColumns[16],
                Units = ParseUnits(filteredColumns[17]),
                Area = double.Parse(filteredColumns[18]),
                Height = double.Parse(filteredColumns[20]),
                AnalyteAnnotation = filteredColumns[21] == "N/A" ? string.Empty : filteredColumns[21],
                NominalConcentration = filteredColumns[22] == "N/A" ? 0d : double.Parse(filteredColumns[22]),
                RetentionTime = double.Parse(filteredColumns[23]),
                ExpectedRetentionTime = double.Parse(filteredColumns[24]),
                RetentionTimeWindow = double.Parse(filteredColumns[25]),
                CentroidLocation = double.Parse(filteredColumns[26]),
                StartScan = int.Parse(filteredColumns[27]),
                StartTime = double.Parse(filteredColumns[28]),
                EndScan = int.Parse(filteredColumns[29]),
                EndTime = double.Parse(filteredColumns[30]),
                IntegrationType = filteredColumns[31],
                SignalToNoiseRatio = filteredColumns[32] == "N/A" ? null : double.Parse(filteredColumns[32]),
                PeakWidth = double.Parse(filteredColumns[33]),
                StandardQueryStatus = filteredColumns[34] == "N/A" ? string.Empty : filteredColumns[34],
                AnalyteTransitionMRM = ParseTransition(filteredColumns[35].Split(' ')[0]),
                AnalyteToISAreaRatio = filteredColumns[37] == "#DIV/0!" ? null : double.Parse(filteredColumns[37]),
                AnalyteToISHeightRatio = filteredColumns[38] == "#DIV/0!" ? null : double.Parse(filteredColumns[38]),
                AnalytePeakWidthAtHalfHeight = double.Parse(filteredColumns[41]),
                AnalyteSlopeOfBaseline = filteredColumns[42] == "#DIV/0!" ? 0d : double.Parse(filteredColumns[42]),
                AnalyteProcessingAlgorithm = filteredColumns[43],
                AnalytePeakAsymmetry = double.Parse(filteredColumns[44]),
                ISPeakName = filteredColumns[45],
                ISUnits = ParseUnits(filteredColumns[46]),
                ISArea = double.Parse(filteredColumns[47]),
                ISHeight = double.Parse(filteredColumns[49]),
                ISConcentration = double.Parse(filteredColumns[51]),
                ISRetentionTime = double.Parse(filteredColumns[52]),
                ISExpectedRetentionTime = double.Parse(filteredColumns[53]),
                ISRetentionTimeWindow = double.Parse(filteredColumns[54]),
                ISCentroidLocation = double.Parse(filteredColumns[55]),
                ISStartScan = int.Parse(filteredColumns[56]),
                ISStartTime = double.Parse(filteredColumns[57]),
                ISStopScan = int.Parse(filteredColumns[58]),
                ISEndTime = double.Parse(filteredColumns[59]),
                ISIntegrationType = filteredColumns[60],
                ISSignalToNoiseRatio = filteredColumns[61] == "N/A" ? 0d : double.Parse(filteredColumns[61]),
                ISPeakWidth = double.Parse(filteredColumns[62]),
                ISTransitionMRM = ParseTransition(filteredColumns[63].Split(' ')[0]),
                ISPeakWidthAtHalfHeight = double.Parse(filteredColumns[66]),
                ISSlopeOfBaseline = double.Parse(filteredColumns[67]),
                ISProcessingAlgorithm = filteredColumns[68],
                ISPeakAsymmetry = double.Parse(filteredColumns[69]),
                UseRecord = filteredColumns[70] == "1",
                RecordModified = filteredColumns[71] == "1",
                CalculatedConcentration = filteredColumns[72] == "#DIV/0!" || filteredColumns[72] == "No Peak" ? 0d : double.Parse(filteredColumns[72]),
                RelativeRetentionTime = double.Parse(filteredColumns[74]),
                Accuracy = filteredColumns[76] == "N/A" ? null : double.Parse(filteredColumns[75]),
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
                "None" => WeightingFactor.Unweighted,
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

        private static List<int> ProcessDataRowHeaders(string headerRow)
        {
            var dataRowHeaders = new HashSet<string>();
            var ignoredColumnIndices = new List<int>();
            string[] headers = headerRow.Split('\t');

            int validHeaderIndex = 0;
            string[] validHeaderFormat =
            {
                "Sample Name",
                "Sample ID",
                "Filetype",
                "Sample Desc.",
                "Set Number",
                "Acquisition Method",
                "Date Of Acq.",
                "Rack Type",
                "Rack Number",
                "Vial Position",
                "Plate Type",
                "Plate Number",
                "Filename",
                "Dil.Factor",
                "Weight To Volume Ratio",
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
                string.Empty,
            };

            for (int i = 0; i < headers.Length; i++)
            {
                string currentHeader = headers[i];
                if (dataRowHeaders.Contains(currentHeader))
                {
                    // Duplicate header columns should be ignored
                    ignoredColumnIndices.Add(i);
                    continue;
                }

                if (validHeaderFormat[validHeaderIndex] != currentHeader)
                {
                    throw new FileFormatException("Data Row headers in export do not match expected format");
                }

                validHeaderIndex++;
                dataRowHeaders.Add(currentHeader);
            }

            return ignoredColumnIndices;
        }
    }
}
