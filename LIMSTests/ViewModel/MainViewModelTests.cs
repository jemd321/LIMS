using Microsoft.VisualStudio.TestTools.UnitTesting;
using LIMS.ViewModel;
using Moq;
using LIMS.Data;
using LIMSTests.Extensions;
using LIMS.Factory;
using LIMS.Model;
using System.IO.Abstractions.TestingHelpers;
using LIMS.Dialog;
using System.Collections.ObjectModel;

namespace LIMS.ViewModel.Tests
{
    [TestClass()]
    public class MainViewModelTests
    {
        private MainViewModel _mainViewModel = default!;
        private RegressionViewModel _regressionViewModel = default!;
        private FileDataService _fileDataService = default!;
        private MockFileSystem _mockfileSystem = default!;
        private Mock<IDialogService> _dialogService = default!;

        [TestInitialize]
        public void SetupTest()
        {
            _regressionViewModel = SetupRegressionViewModel();
            _mockfileSystem = new MockFileSystem();
            _fileDataService = new FileDataService(_mockfileSystem);
            _dialogService = new Mock<IDialogService>();
            _mainViewModel = new MainViewModel(_regressionViewModel, _fileDataService, _dialogService.Object);
        }

        private RegressionViewModel SetupRegressionViewModel()
        {
            var mockRegressionDataProvider = new Mock<IRegressionDataProvider>();
            var mockRegressionFactory = new Mock<IRegressionFactory>();
            return new RegressionViewModel(mockRegressionDataProvider.Object, mockRegressionFactory.Object);
        }

        [TestMethod()]
        public void SelectedViewModel_OnVMChanged_FiresPropertyChangedEvent()
        {
            var fired = _mainViewModel.IsPropertyChangedFired(() =>
            {
                _mainViewModel.SelectedViewModel = _regressionViewModel;
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
            _dialogService.Verify(m => m.ShowDialog<ProjectCreationDialogViewModel>(It.IsAny<Action<string>>()), Times.Once);
        }
    }
}