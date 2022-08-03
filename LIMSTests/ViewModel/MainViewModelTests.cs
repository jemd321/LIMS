using Microsoft.VisualStudio.TestTools.UnitTesting;
using LIMS.ViewModel;
using Moq;
using LIMS.Data;
using LIMSTests.Extensions;
using LIMS.Factory;
using LIMS.Model;

namespace LIMS.ViewModel.Tests
{
    [TestClass()]
    public class MainViewModelTests
    {
        private MainViewModel _mainViewModel = default!;
        private RegressionViewModel _regressionViewModel = default!;
        private Mock<IFileDataService> _fileDataService = default!;

        [TestInitialize]
        public void SetupTest()
        {
            _regressionViewModel = SetupRegressionViewModel();
            _fileDataService = new Mock<IFileDataService>();
            _mainViewModel = new MainViewModel(_regressionViewModel, _fileDataService.Object);
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
        public void Load_WhenCalled_LoadsProjectList()
        {
            
            
            
            Assert.Fail();
        }
    }
}