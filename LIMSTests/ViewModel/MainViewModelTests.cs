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
        private Mock<DataImporter> _dataImporter = default!;

        [TestInitialize]
        public void SetupTest()
        {
            _regressionViewModel = SetupRegressionViewModel();
            _dataImporter = new Mock<DataImporter>();
            _mainViewModel = new MainViewModel(_regressionViewModel, _dataImporter.Object);
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
    }
}