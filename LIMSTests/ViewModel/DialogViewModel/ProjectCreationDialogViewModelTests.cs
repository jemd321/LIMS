using LIMS.Data;
using System.IO.Abstractions.TestingHelpers;
using LIMS.Model;
using Moq;
using System.Linq;
using LIMSTests.Extensions;
using LIMS.ViewModel.DialogViewModel;

namespace LIMS.ViewModel.Tests
{
    [TestClass()]
    public class ProjectCreationDialogViewModelTests
    {
        ProjectCreationDialogViewModel _viewModel = default!;
        Mock<IDataProvider> _mockFileDataService = default!;
        FileDataProvider _fileDataService = default!;
        MockFileSystem _mockFileSystem = default!;


        [TestInitialize()]
        public void SetupTestWithMockFileDataService()
        {
            _mockFileSystem = new MockFileSystem();
            _mockFileDataService = new Mock<IDataProvider>();
            _mockFileDataService.Setup(m => m.LoadProjects()).Returns(new System.Collections.ObjectModel.ObservableCollection<Project> { });
            _viewModel = new ProjectCreationDialogViewModel(_mockFileDataService.Object);
        }

        public void SetupTestForMockFileSystem()
        {
            _mockFileSystem = new MockFileSystem();
            _fileDataService = new FileDataProvider(_mockFileSystem);
            _viewModel = new ProjectCreationDialogViewModel(_fileDataService);
        }

        private Project SetupTestProject(string projectName)
        {
            var expectedProject = new Project(projectName);
            var expectedAppDataRoaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var expectedAppDirectory = Path.Combine(expectedAppDataRoaming, "LIMS");
            var expectedProjectsDirectory = Path.Combine(expectedAppDirectory, "Projects");

            _mockFileSystem.AddDirectory(expectedProjectsDirectory);
            _mockFileSystem.AddDirectory(Path.Combine(expectedProjectsDirectory, projectName));
            return expectedProject;
        }

        [TestMethod()]
        public void Load_WhenVMLoaded_GetsProjects()
        {
            const string TESTPROJECTNAME = "test";
            SetupTestForMockFileSystem();
            Project expectedProject = SetupTestProject(TESTPROJECTNAME);

            _viewModel.Load();
            var actualProjectsList = _viewModel.LoadedProjects;

            Assert.IsTrue(_viewModel.LoadedProjects.Count == 1);
            Assert.AreEqual(expectedProject.ProjectID, actualProjectsList.Single().ProjectID);
        }

        [TestMethod()]
        public void NewProjectName_RaisesPropertyChanged_WhenPropertyChanged()
        {
            _viewModel.Load();
            var fired = _viewModel.IsPropertyChangedFired(() =>
            {
                _viewModel.NewProjectName = "changed";
            }, nameof(_viewModel.NewProjectName));

            Assert.IsTrue(fired);
        }

        [TestMethod()]
        public void NewProjectName_WhenTooLong_AddsValidationError()
        {
            _viewModel.Load();
            _viewModel.NewProjectName = "thisstringisalittlemorethan36characterslong";
            List<string> errorList = _viewModel.GetErrorsAsList(nameof(_viewModel.NewProjectName));
            const string EXPECTEDERRORMESSAGE = "Project name is too long";

            Assert.IsTrue(_viewModel.HasErrors);
            Assert.IsTrue(errorList.Count() == 1);
            Assert.AreEqual(EXPECTEDERRORMESSAGE, errorList.SingleOrDefault());
        }

        [TestMethod()]
        public void NewProjectName_WhenNoLongerTooLong_ClearsError()
        {
            _viewModel.Load();
            _viewModel.NewProjectName = "thisstringisalittlemorethan36characterslong";
            _viewModel.NewProjectName = "nowvalid";
            List<string> errorList = _viewModel.GetErrorsAsList(nameof(_viewModel.NewProjectName));

            Assert.IsFalse(_viewModel.HasErrors);
            Assert.IsFalse(errorList.Any());
        }

        [TestMethod()]
        public void NewProjectName_WhenInvalidCharacter_AddsValidationError()
        {
            _viewModel.Load();
            _viewModel.NewProjectName = "?";
            List<string> errorList = _viewModel.GetErrorsAsList(nameof(_viewModel.NewProjectName));
            const string EXPECTEDERRORMESSAGE = "Project name cannot contain: < > \\ / \" : | ? * .";

            Assert.IsTrue(_viewModel.HasErrors);
            Assert.IsTrue(errorList.Count() == 1);
            Assert.AreEqual(EXPECTEDERRORMESSAGE, errorList.SingleOrDefault());
        }

        [TestMethod()]
        public void NewProjectName_WhenInvalidCharacterCleared_ClearsError()
        {
            _viewModel.Load();
            _viewModel.NewProjectName = "?";
            _viewModel.NewProjectName = "nowvalid";
            List<string> errorList = _viewModel.GetErrorsAsList(nameof(_viewModel.NewProjectName));

            Assert.IsFalse(_viewModel.HasErrors);
            Assert.IsFalse(errorList.Any());
        }

        [TestMethod()]
        public void NewProjectName_WhenProjectExists_AddsProjectError()
        {
            const string TESTPROJECTNAME = "test";
            SetupTestForMockFileSystem();
            Project expectedProject = SetupTestProject(TESTPROJECTNAME);
            _viewModel.Load();
            _viewModel.NewProjectName = TESTPROJECTNAME;
            List<string> errorList = _viewModel.GetErrorsAsList(nameof(_viewModel.NewProjectName));
            const string EXPECTEDERRORMESSAGE = "Project already exists";

            Assert.IsTrue(_viewModel.HasErrors);
            Assert.IsTrue(errorList.Count() == 1);
            Assert.AreEqual(EXPECTEDERRORMESSAGE, errorList.SingleOrDefault());
        }

        [TestMethod()]
        public void CreateProjectCommand_CanExecute_WhenNewProjectNameEntered()
        {
            _viewModel.Load();
            _viewModel.NewProjectName = "valid";
            Assert.IsTrue(_viewModel.CreateProjectCommand.CanExecute(null));
        }

        [TestMethod()]
        public void CreateProjectCommand_CannotExecute_WhenNewProjectNameIsNullOrEmpty()
        {
            _viewModel.Load();
            _viewModel.NewProjectName = "";
            Assert.IsFalse(_viewModel.CreateProjectCommand.CanExecute(null));
        }

        [TestMethod()]
        public void CreateProjectCommand_CannotExecute_WhenNewProjectNameIsTooLong()
        {
            _viewModel.Load();
            _viewModel.NewProjectName = "thisstringisalittlemorethan36characterslong";
            Assert.IsFalse(_viewModel.CreateProjectCommand.CanExecute(null));
        }

        [TestMethod()]
        public void CreateProjectCommand_CannotExecute_WhenNewProjectNameContainsInvalidCharacter()
        {
            _viewModel.Load();
            _viewModel.NewProjectName = "?";
            Assert.IsFalse(_viewModel.CreateProjectCommand.CanExecute(null));
        }

        [TestMethod()]
        public void CreateProjectCommand_CannotExecute_WhenNewProjectNameAlreadyExists()
        {
            _viewModel.Load();
            _viewModel.LoadedProjects.Add(new Project("test"));
            _viewModel.NewProjectName = "test";
            Assert.IsFalse(_viewModel.CreateProjectCommand.CanExecute(null));
        }

        [TestMethod()]
        public void SelectedProject_RaisesPropertyChanged_WhenPropertyChanged()
        {
            _viewModel.Load();
            var fired = _viewModel.IsPropertyChangedFired(() =>
            {
                _viewModel.SelectedProject = null;
            }, nameof(_viewModel.SelectedProject));

            Assert.IsTrue(fired);
        }

        [TestMethod()]
        public void DeleteProjectCommand_WhenExecuted_MakesCallToFileDataService()
        {
            _viewModel.Load();
            _viewModel.SelectedProject = new Project("test");
            _viewModel.DeleteProjectCommand.Execute(null);

            _mockFileDataService.Verify(fds => fds.DeleteProject(It.IsAny<Project>()), Times.Once());
        }

        [TestMethod()]
        public void DeleteProjectCommand_CanExecute_WhenProjectSelected()
        {
            _viewModel.Load();
            _viewModel.SelectedProject = new Project("test");
            Assert.IsTrue(_viewModel.DeleteProjectCommand.CanExecute(null));
        }

        [TestMethod()]
        public void DeleteProjectCommand_CannotExecute_WhenNoProjectSelected()
        {
            _viewModel.Load();
            _viewModel.SelectedProject = null;
            Assert.IsFalse(_viewModel.DeleteProjectCommand.CanExecute(null));
        }
    }
}