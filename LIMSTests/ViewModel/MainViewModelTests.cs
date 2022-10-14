using System.IO.Abstractions.TestingHelpers;
using LIMS.Data;
using LIMS.Dialog;
using LIMS.Model;
using LIMS.ViewModel.DialogViewModel;
using LIMSTests.Extensions;
using Moq;

namespace LIMS.ViewModel.Tests
{
    [TestClass()]
    public class MainViewModelTests
    {
        private MainViewModel _mainViewModel = default!;
        private Mock<IRegressionViewModel> _regressionViewModelMock = default!;
        private FileDataService _fileDataService = default!;
        private MockFileSystem _mockfileSystem = default!;
        private Mock<IDataImporter> _dataImporterMock = default!;
        private Mock<IDialogService> _dialogServiceMock = default!;

        [TestInitialize]
        public void SetupTest()
        {
            _regressionViewModelMock = new Mock<IRegressionViewModel>();
            _mockfileSystem = new MockFileSystem();
            _fileDataService = new FileDataService(_mockfileSystem);
            _dataImporterMock = new Mock<IDataImporter>();
            _dialogServiceMock = new Mock<IDialogService>();
            _mainViewModel = new MainViewModel(_regressionViewModelMock.Object, _fileDataService, _dataImporterMock.Object, _dialogServiceMock.Object);
        }

        [TestMethod()]
        public void SelectedViewModel_OnVMChanged_FiresPropertyChangedEvent()
        {
            var fired = _mainViewModel.IsPropertyChangedFired(() =>
            {
                _mainViewModel.SelectedViewModel = _regressionViewModelMock.Object as ViewModelBase;
            }, nameof(_mainViewModel.SelectedViewModel));

            Assert.IsTrue(fired);
        }

        [TestMethod()]
        public void Load_WhenNoProjects_LoadsEmptyProjectList()
        {
            _mainViewModel.Load();

            Assert.IsTrue(_mainViewModel.Projects.Count == 0);
        }

        [TestMethod()]
        public void Load_WhenProjectsExist_LoadsProjectList()
        {
            var expectedProject = new Project("Test");
            var expectedAppDataRoaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var expectedAppDirectory = Path.Combine(expectedAppDataRoaming, "LIMS");
            var expectedProjectsDirectory = Path.Combine(expectedAppDirectory, "Projects");

            _mockfileSystem.AddDirectory(expectedProjectsDirectory);
            _mockfileSystem.AddDirectory(Path.Combine(expectedProjectsDirectory, "Test"));

            _mainViewModel.Load();
            var actualProjectsList = _mainViewModel.Projects;

            Assert.IsTrue(_mainViewModel.Projects.Count == 1);
            Assert.AreEqual(expectedProject.ProjectID, actualProjectsList.Single().ProjectID);
        }

        [TestMethod()]
        public void CreateNewProjectCommand_WhenInvoked_ShowsDialog()
        {
            _mainViewModel.Load();

            _mainViewModel.CreateNewProjectCommand.Execute(null);
            _dialogServiceMock.Verify(m => m.ShowActionDialog<ProjectEditDialogViewModel>(It.IsAny<Action<bool>>()), Times.Once);
        }
    }
}