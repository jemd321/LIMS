using LIMS.Factory;
using LIMS.Model.RegressionModels;
using LIMSTests.Extensions;
using Moq;

namespace LIMS.ViewModel.Tests
{
    [TestClass]
    public class RegressionViewModelTests
    {
        private RegressionViewModel _regressionViewModel = default!;
        private Mock<IRegressionDataViewModel> _regressionDataViewModel = default!;
        private Mock<IRegressionFactory> _regressionFactoryMock = default!;
        private Mock<Regression> _regression = default!;

        [TestInitialize]
        public void SetupTest()
        {
            _regressionFactoryMock = new Mock<IRegressionFactory>();
            _regressionViewModel = new RegressionViewModel(_regressionFactoryMock.Object);
            _regressionDataViewModel = new Mock<IRegressionDataViewModel>();
            _regression = new Mock<Regression>();
        }

        [TestMethod]
        public void Load_WhenRegressionDataViewModelCreated_FiresPropertyChangedEvent()
        {
            var fired = _regressionViewModel.IsPropertyChangedFired(
                () =>
            {
                _regressionViewModel.RegressionDataViewModel = _regressionDataViewModel.Object;
            },
                nameof(_regressionViewModel.RegressionDataViewModel));

            Assert.IsTrue(fired);
        }
    }
}