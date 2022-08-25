using LIMS.Factory;
using LIMS.Model.RegressionModels;
using LIMSTests.Extensions;
using Moq;

namespace LIMS.ViewModel.Tests
{
    [TestClass()]
    public class RegressionViewModelTests
    {
        RegressionViewModel _regressionViewModel = default!;
        Mock<IRegressionDataViewModel> _regressionDataViewModel = default!;
        readonly Mock<IRegressionFactory> _regressionFactoryMock = default!;
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