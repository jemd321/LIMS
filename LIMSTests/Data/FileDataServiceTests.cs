using Microsoft.VisualStudio.TestTools.UnitTesting;
using LIMS.Data;
using LIMS.Model.RegressionModels;
using LIMS.Model;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;

namespace LIMS.Data.Tests
{
    [TestClass()]
    public class FileDataServiceTests
    {
        private MockFileSystem _mockfileSystem = default!;
        private FileDataService _fileDataService = default!;
        private RegressionData _regressionData = default!;

        private string _expectedAppDataRoaming = default!;
        private string _expectedAppDirectory = default!;
        private string _expectedProjectsDirectory = default!;



        [TestInitialize]
        public void TestSetup()
        {
            _mockfileSystem = new MockFileSystem();
            _fileDataService = new FileDataService(_mockfileSystem);
            _regressionData = SetupRegressionData();

            _expectedAppDataRoaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _expectedAppDirectory = Path.Combine(_expectedAppDataRoaming, "LIMS");
            _expectedProjectsDirectory = Path.Combine(_expectedAppDirectory, "Projects");
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

        [TestMethod()]
        public void SetupApplicationStorage_WhenNotSetup_CreatesNewFolders()
        {
            _mockfileSystem.AddDirectory(_expectedAppDataRoaming);

            _fileDataService.SetupApplicationStorage();

            Assert.IsTrue(_mockfileSystem.Directory.Exists(_expectedAppDirectory));
            Assert.IsTrue(_mockfileSystem.Directory.Exists(_expectedProjectsDirectory));
        }

        [TestMethod()]
        public void SetupApplicationStorage_WhenAlreadySetup_KeepsDirectoriesAsIs()
        {
            _mockfileSystem.AddDirectory(_expectedAppDataRoaming);
            _mockfileSystem.AddDirectory(_expectedAppDirectory);
            _mockfileSystem.AddDirectory(_expectedProjectsDirectory);

            _fileDataService.SetupApplicationStorage();

            Assert.IsTrue(_mockfileSystem.Directory.Exists(_expectedAppDirectory));
            Assert.IsTrue(_mockfileSystem.Directory.Exists(_expectedProjectsDirectory));
        }

        [TestMethod()]
        public void ApplicationDirectory_Returns_CorrectDirectory()
        {
            string actualAppDirectory = _fileDataService.ApplicationDirectory;

            Assert.AreEqual(_expectedAppDirectory, actualAppDirectory);
        }

        [TestMethod()]
        public void ProjectsDirectory_Returns_CorrectDirectory()
        {
            string actualProjectsDirectory = _fileDataService.ProjectsDirectory;

            Assert.AreEqual(_expectedProjectsDirectory, actualProjectsDirectory);
        }

        [TestMethod()]
        public void LoadProjects_WhenNoProjects_ReturnsEmptyList()
        {
            _mockfileSystem.AddDirectory(_expectedProjectsDirectory);
            
            var loadedProjects = _fileDataService.LoadProjects();

            Assert.IsFalse(loadedProjects.Any());
        }

        [TestMethod()]
        public void LoadProjects_WhenProjectsExist_ReturnsLoadedProject()
        {
            var expectedProject = new Project("Test");
            _mockfileSystem.AddDirectory(_expectedProjectsDirectory);
            _mockfileSystem.AddDirectory(Path.Combine(_expectedProjectsDirectory, "Test"));

            var loadedProjects = _fileDataService.LoadProjects();

            Assert.IsTrue(loadedProjects.Count == 1);
            Assert.IsTrue(expectedProject.ProjectID == loadedProjects.First().ProjectID);
        }

        [TestMethod()]
        public void LoadProjects_WhenProjectsWithRunsExist_ReturnsLoadedProject()
        {
            var expectedProject = new Project("Test");
            expectedProject.AnalyticalRuns.Add("TestRun", new AnalyticalRun("TestRun", "Test"));

            _mockfileSystem.AddDirectory(_expectedProjectsDirectory);
            _mockfileSystem.AddDirectory(Path.Combine(_expectedProjectsDirectory, "Test"));
            _mockfileSystem.AddDirectory(Path.Combine(_expectedProjectsDirectory, "Test\\TestRun"));

            var loadedProjects = _fileDataService.LoadProjects();
            var actualProject = loadedProjects.First();
            var actualAnalyticalRun = actualProject.AnalyticalRuns.First();

            Assert.IsTrue(actualProject.AnalyticalRuns.Count == 1);
            Assert.IsTrue(actualAnalyticalRun.Key == expectedProject.AnalyticalRuns.First().Value.AnalyticalRunID);
            Assert.IsTrue(actualAnalyticalRun.Value.ParentProjectID == expectedProject.ProjectID);
        }

        [TestMethod()]
        public void LoadRunTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SaveRunTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ValidateFilePathTest1()
        {
            Assert.Fail();
        }
    }
}