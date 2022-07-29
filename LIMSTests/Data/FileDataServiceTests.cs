using LIMS.Model.RegressionModels;
using LIMS.Model;

namespace LIMS.Data.Tests
{
    [TestClass()]
    public class FileDataServiceTests
    {
        private FileDataService _fileDataService = default!;
        private RegressionData _regressionData = default!;

        [TestInitialize]
        public void TestSetup()
        {
            _fileDataService = new FileDataService();
            _regressionData = SetupRegressionData();
        }

        private RegressionData SetupRegressionData()
        {

            var testStandards = new List<Standard>()
            {
                new Standard { NominalConcentration= 0.0, InstrumentResponse = 0.0, SampleName = "Zero"},
                new Standard { NominalConcentration = 0.1, InstrumentResponse = 12.36, SampleName = "F"},
                new Standard { NominalConcentration = 0.2, InstrumentResponse = 24.83, SampleName = "E"},
                new Standard { NominalConcentration = 0.3, InstrumentResponse = 35.91, SampleName = "D"},
                new Standard { NominalConcentration = 0.4, InstrumentResponse = 48.79, SampleName = "C"},
                new Standard { NominalConcentration = 0.5, InstrumentResponse = 60.42, SampleName = "B"},
            };

            var testQCs = new List<QualityControl>()
            {
                new QualityControl { NominalConcentration = 0.35, InstrumentResponse = 40.0, SampleName="MQC" }
            };

            var testUnknowns = new List<Unknown>()
            {
                new Unknown {InstrumentResponse = 20.0, SampleName="001"}
            };

            return new RegressionData()
            {
                Standards = testStandards,
                QualityControls = testQCs,
                Unknowns = testUnknowns
            };
        }

        [TestMethod()]
        public void ValidateFilePathTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetRawDataTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void LoadRun_GivenRunInfo_GetsRegressionData()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SaveRun_GivenRegressionData_SavesAsJSon()
        {
            _fileDataService.SaveRun(_regressionData);

            Assert.Fail();

        }
    }
}