using LIMS.Model;
using LIMS.Factory;
using LIMSTests.Extensions;
using Moq;
using LIMS.Model.RegressionModels;

namespace LIMS.ViewModel.Tests
{
    [TestClass()]
    public class RegressionViewModelTests
    {
        RegressionViewModel _regressionViewModel = default!;
        Mock<IRegressionDataViewModel> _regressionDataViewModel = default!;
        Mock<IRegressionFactory> _regressionFactoryMock = default!;
        Mock<Regression> _regression = default!;

        [TestInitialize]
        public void SetupTest()
        {
            _regressionViewModel = new RegressionViewModel(_regressionFactoryMock.Object);
            _regressionDataViewModel = new Mock<IRegressionDataViewModel>();
            _regression = new Mock<Regression>();
        }
        [TestMethod()]
        public void Load_WhenRegressionDataViewModelCreated_FiresPropertyChangedEvent()
        {
            var fired = _regressionViewModel.IsPropertyChangedFired(() =>
            {
                _regressionViewModel.RegressionDataViewModel = _regressionDataViewModel.Object;
            }, nameof(_regressionViewModel.RegressionDataViewModel));

            Assert.IsTrue(fired);
        }
    }
}