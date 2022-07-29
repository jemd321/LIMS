using LIMS.Data;
using LIMS.Model;
using LIMS.Enums;

namespace LIMSTests.Data
{
    [TestClass]
    public class AnalystExportParserTests
    {
        [TestMethod]
        public void ReadAnalystExport_GivenFilePath_ReturnsParsedExport()
        {
            var analystDataProvider = new AnalystDataProvider();
            string sampleAnalystExport = Properties.Resources.SampleAnalystExport;
            // to add expected object
            var peakInfo = new List<AnalystExportHeaderPeakInfo>();
            var internalStandardPeakInfo = new AnalystExportHeaderPeakInfo()
            {
                PeakName = "Itraconazole-d9",
                IsInternalStandard = true,
                InternalStandard = null,
                TransitionMRM = new TransitionMRM()
                {
                    Q1 = 714.50,
                    Q3 = 401.30
                }
            };
            var analytePeakInfo = new AnalystExportHeaderPeakInfo()
            {
                PeakName = "Itraconazole",
                IsInternalStandard = false,
                InternalStandard = "Itraconazole-d9",
                TransitionMRM = new TransitionMRM()
                {
                    Q1 = 705.20,
                    Q3 = 392.30
                }
            };
            peakInfo.Add(internalStandardPeakInfo);
            peakInfo.Add(analytePeakInfo);

            var headerInfo = new AnalystExportHeaderRegressionInfo()
            {
                Regression = RegressionType.Quadratic,
                WeightingFactor = WeightingFactor.OneOverXSquared,
                A = 0.00117,
                B = 0.0021,
                C = 9.2e-008,
                RSquared = 0.9967
            };

            var dataRows = new List<AnalystExportRow>();
            var dataRow1 = new AnalystExportRow()
            {
                SampleName = "166 001 996183 SST-LLOQ 1",
                SampleID = 1,
                SampleType = SampleType.Unknown,
                SampleDescription = "",
                SetNumber = 0,
                AcquisitonMethod = "996183-S15-2022-04-25 A2B2C2.dam",
                AcquisitionDate = new DateTime(2022, 4, 26, 3, 47, 28),
                RackType = "Sample Manager w/ SO",
                RackNumber = 1,
                VialPosition = 12,
                PlateType = "Roundwell",
                PlateNumber = 5,
                FileName = @"Run 166\996183-S15-166-001.wiff",
                DilutionFactor = 1,
                WeightToVolumeRatio = 0,
                SampleAnnotation1 = "",
                SampleAnnotation2 = "",
                PeakName = "Itraconazole",
                Units = Units.ng_mL,
                Area = 153.4,
                Height = 86.7,
                AnalyteAnnotation = "",
                NominalConcentration = 0d,
                RetentionTime = 1.78,
                ExpectedRetentionTime = 1.78,
                RetentionTimeWindow = 10,
                CentroidLocation = 1.78,
                StartScan = 496,
                StartTime = 1.73,
                EndScan = 523,
                EndTime = 1.83,
                IntegrationType = "Base To Base",
                SignalToNoiseRatio = null,
                PeakWidth = 0.0945,
                StandardQueryStatus = "",
                AnalyteTransitionMRM = new TransitionMRM()
                {
                    Q1 = 705.200,
                    Q3 = 392.300
                },
                AnalyteToISAreaRatio = 0.0102,
                AnalyteToISHeightRatio = 0.0155,
                AnalytePeakWidthAtHalfHeight = 0.0201,
                AnalyteSlopeOfBaseline = 0d,
                AnalyteProcessingAlgorithm = "Specify Parameters - MQIII",
                AnalytePeakAsymmetry = 1.07,
                ISPeakName = "Itraconazole-d9",
                ISUnits = Units.ng_mL,
                ISHeight = 5580d,
                ISArea = 15015.6,
                ISConcentration = 1d,
                ISRetentionTime = 1.76,
                ISExpectedRetentionTime = 1.76,
                ISRetentionTimeWindow = 10d,
                ISCentroidLocation = 1.76,
                ISStartScan = 474,
                ISStartTime = 1.66,
                ISStopScan = 534,
                ISEndTime = 1.87,
                ISIntegrationType = "Base To Base",
                ISSignalToNoiseRatio = 0d,
                ISPeakWidth = 0.21,
                ISTransitionMRM = new TransitionMRM()
                {
                    Q1 = 714.500,
                    Q3 = 401.300
                },
                ISPeakWidthAtHalfHeight = 0.0399,
                ISSlopeOfBaseline = 0d,
                ISProcessingAlgorithm = "Specify Parameters - MQIII",
                ISPeakAsymmetry = 0.998,
                UseRecord = false,
                RecordModified = false,
                CalculatedConcentration = 4.31,
                RelativeRetentionTime = 1.01,
                Accuracy = null,
                ResponseFactor1 = null,
                ResponseFactor2 = 0.936,
                ResponseFactor3 = 1d,
            };
            dataRows.Add(dataRow1);

            var expected = new AnalystExport()
            {
                Peaks = peakInfo,
                RegressionInfo = headerInfo,
                DataRows = dataRows
            };

            var actual = analystDataProvider.ParseAnalystExport(ref sampleAnalystExport);

            Assert.AreEqual(expected.Peaks[0], actual.Peaks[0]);
            Assert.AreEqual(expected.Peaks[1], actual.Peaks[1]);
            Assert.AreEqual(expected.RegressionInfo, actual.RegressionInfo);
            Assert.AreEqual(expected.DataRows[0], actual.DataRows[0]);

        }
    }
}