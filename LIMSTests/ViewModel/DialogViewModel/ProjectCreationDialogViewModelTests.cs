using Microsoft.VisualStudio.TestTools.UnitTesting;
using LIMS.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LIMS.Data;
using System.IO.Abstractions.TestingHelpers;
using LIMS.Model;

namespace LIMS.ViewModel.Tests
{
    [TestClass()]
    public class ProjectCreationDialogViewModelTests
    {
        ProjectCreationDialogViewModel _viewModel = default!;
        FileDataService _fileDataService = default!;
        MockFileSystem _mockFileSystem = default!;


        [TestInitialize()]
        public void SetupTest()
        {
            _mockFileSystem = new MockFileSystem();
            _fileDataService = new FileDataService(_mockFileSystem);
            _viewModel = new ProjectCreationDialogViewModel(_fileDataService);
        }

        [TestMethod()]
        public void CreateProjectCommand()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void Load_WhenVMLoaded_GetsProjects()
        {
            var expectedProject = new Project("Test");
            var expectedAppDataRoaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var expectedAppDirectory = Path.Combine(expectedAppDataRoaming, "LIMS");
            var expectedProjectsDirectory = Path.Combine(expectedAppDirectory, "Projects");

            _mockFileSystem.AddDirectory(expectedProjectsDirectory);
            _mockFileSystem.AddDirectory(Path.Combine(expectedProjectsDirectory, "Test"));

            _viewModel.Load();
            var actualProjectsList = _viewModel.LoadedProjects;

            Assert.IsTrue(_viewModel.LoadedProjects.Count == 1);
            Assert.AreEqual(expectedProject.ProjectID, actualProjectsList.Single().ProjectID);
        }
    }
}