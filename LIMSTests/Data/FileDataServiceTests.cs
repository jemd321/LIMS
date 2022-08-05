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
        private RegressionData _mockRegressionData = default!;

        private string _expectedAppDataRoaming = default!;
        private string _expectedAppDirectory = default!;
        private string _expectedProjectsDirectory = default!;



        [TestInitialize]
        public void TestSetup()
        {
            _mockfileSystem = new MockFileSystem();
            _fileDataService = new FileDataService(_mockfileSystem);
            _mockRegressionData = SetupRegressionData();

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
        public void LoadAnalyticalRun_GivenAnalyticalRunID_GetsAnalyticalRun()
        {
            string testProjectDirectory = Path.Combine(_expectedProjectsDirectory, "Test", "TestRun");
            _mockfileSystem.AddDirectory(testProjectDirectory);
            _fileDataService = new FileDataService(_mockfileSystem);

            var mockAnalyticalRun = new AnalyticalRun("TestRun", "Test", _mockRegressionData);
            _fileDataService.SaveAnalyticalRun(mockAnalyticalRun);
            var testProject = new Project("Test");
            const string TESTANALYTICALRUN = "TestRun";
            testProject.AnalyticalRunIDs.Add(TESTANALYTICALRUN);

            var actualLoadedRun = _fileDataService.LoadAnalyticalRun(testProject, TESTANALYTICALRUN);

            Assert.AreEqual(mockAnalyticalRun.AnalyticalRunID, actualLoadedRun.AnalyticalRunID);
            Assert.AreEqual(mockAnalyticalRun.ParentProjectID, actualLoadedRun.ParentProjectID);
            Assert.IsTrue(actualLoadedRun.RegressionData.Standards.Any());
        }

        [TestMethod()]
        public void LoadAnalyticalRun_GivenIncorrectAnalyticalRunID_Throws()
        {
            string testProjectDirectory = Path.Combine(_expectedProjectsDirectory, "Test", "TestRun");
            _mockfileSystem.AddDirectory(testProjectDirectory);
            _fileDataService = new FileDataService(_mockfileSystem);

            var mockAnalyticalRun = new AnalyticalRun("TestRun", "Test", _mockRegressionData);
            _fileDataService.SaveAnalyticalRun(mockAnalyticalRun);
            var testProject = new Project("Test");
            const string TESTANALYTICALRUN = "IncorrectRun";
            testProject.AnalyticalRunIDs.Add(TESTANALYTICALRUN);

            Assert.ThrowsException<FileNotFoundException>(() => _fileDataService.LoadAnalyticalRun(testProject, TESTANALYTICALRUN));
        }

        [TestMethod()]
        public void SaveAnalyticalRun_GivenRun_SavesAsJsonFile()
        {
            string testProjectDirectory = Path.Combine(_expectedProjectsDirectory, "Test", "TestRun");
            _mockfileSystem.AddDirectory(testProjectDirectory);
            _fileDataService = new FileDataService(_mockfileSystem);

            string expectedSaveFilePath = Path.Combine(testProjectDirectory, "TestRun.json");
            var mockAnalyticalRun = new AnalyticalRun("TestRun", "Test", _mockRegressionData);

            _fileDataService.SaveAnalyticalRun(mockAnalyticalRun);

            Assert.IsTrue(_mockfileSystem.FileExists(expectedSaveFilePath));
        }

        [TestMethod]
        public void SaveAnalyticalRun_GivenExistingRun_OverwritesJsonFile()
        {
            string testProjectDirectory = Path.Combine(_expectedProjectsDirectory, "Test", "TestRun");
            string expectedSaveFilePath = Path.Combine(testProjectDirectory, "TestRun.json");
            _mockfileSystem.AddDirectory(testProjectDirectory);
            _mockfileSystem.AddFile(expectedSaveFilePath, new MockFileData(""));
            _fileDataService = new FileDataService(_mockfileSystem);


            var mockAnalyticalRun = new AnalyticalRun("TestRun", "Test", _mockRegressionData);

            _fileDataService.SaveAnalyticalRun(mockAnalyticalRun);
            string actualFileContent = _mockfileSystem.File.ReadAllText(expectedSaveFilePath);

            Assert.IsTrue(actualFileContent.Length > 0);
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
            var expectedProject = new Project("test");
            _mockfileSystem.AddDirectory(_expectedProjectsDirectory);
            _mockfileSystem.AddDirectory(Path.Combine(_expectedProjectsDirectory, "test"));

            var loadedProjects = _fileDataService.LoadProjects();

            Assert.IsTrue(loadedProjects.Count == 1);
            Assert.IsTrue(expectedProject.ProjectID == loadedProjects.First().ProjectID);
        }

        [TestMethod()]
        public void LoadProjects_WhenProjectsWithRunsExist_ReturnsLoadedProject()
        {
            var expectedProject = new Project("test");
            expectedProject.AnalyticalRunIDs.Add("testrun");

            _mockfileSystem.AddDirectory(_expectedProjectsDirectory);
            _mockfileSystem.AddDirectory(Path.Combine(_expectedProjectsDirectory, "test"));
            _mockfileSystem.AddDirectory(Path.Combine(_expectedProjectsDirectory, "test\\testrun"));

            var loadedProjects = _fileDataService.LoadProjects();
            var actualProject = loadedProjects.First();
            var actualAnalyticalRun = actualProject.AnalyticalRunIDs.First();

            Assert.IsTrue(actualProject.AnalyticalRunIDs.Count == 1);
            Assert.IsTrue(actualAnalyticalRun == expectedProject.AnalyticalRunIDs.First());
        }

        [TestMethod()]
        public void CreateProject_WhenGivenNewProject_CreatesProject()
        {
            var testProject = new Project("test");
            _mockfileSystem.AddDirectory(_expectedProjectsDirectory);
            var expectedProjectDirectory = _mockfileSystem.Path.Combine(_expectedProjectsDirectory, testProject.ProjectID);

            _fileDataService.CreateProject(testProject);

            Assert.IsTrue(_mockfileSystem.Directory.Exists(expectedProjectDirectory));
        }

        [TestMethod()]
        public void CreateProject_WhenGivenExistingProject_DoesNothing()
        {
            var testProject = new Project("test");
            var expectedProjectDirectory = _mockfileSystem.Path.Combine(_expectedProjectsDirectory, testProject.ProjectID);
            _mockfileSystem.AddDirectory(expectedProjectDirectory);
            var expectedDirectoryCount = _mockfileSystem.AllDirectories.Count();

            _fileDataService.CreateProject(testProject);
            var actualDirectoryCount = _mockfileSystem.AllDirectories.Count();

            Assert.AreEqual(expectedDirectoryCount, actualDirectoryCount);

        }

        [TestMethod()]
        public void DeleteProject_GivenExistingProject_DeletesIt()
        {
            var testProject = new Project("test");
            var expectedProjectDirectory = _mockfileSystem.Path.Combine(_expectedProjectsDirectory, testProject.ProjectID);
            _mockfileSystem.AddDirectory(expectedProjectDirectory);

            _fileDataService.DeleteProject(testProject);

            Assert.IsFalse(_mockfileSystem.Directory.Exists(expectedProjectDirectory));
        }

        [TestMethod()]
        public void DeleteProject_ProjectNotFound_DoesNothing()
        {
            var testProject = new Project("test");
            _mockfileSystem.AddDirectory(_expectedProjectsDirectory);
            var expectedDirectoryCount = _mockfileSystem.AllDirectories.Count();

            _fileDataService.DeleteProject(testProject);
            var actualDirectoryCount = _mockfileSystem.AllDirectories.Count();

            Assert.AreEqual(expectedDirectoryCount, actualDirectoryCount);
        }
    }
}