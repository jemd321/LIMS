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
        Mock<IRegressionDataProvider> _regressionDataProvider = default!;
        Mock<IRegressionFactory> _regressionFactory = default!;
        Mock<Regression> _regression = default!;

        [TestInitialize]
        public void SetupTest()
        {
            _regressionDataProvider = new Mock<IRegressionDataProvider>();
            _regressionFactory = new Mock<IRegressionFactory>();
            _regressionViewModel = new RegressionViewModel(_regressionDataProvider.Object, _regressionFactory.Object);
        }
        [TestMethod()]
        public void Load_WhenRegressionDataViewModelCreated_FiresPropertyChangedEvent()
        {
            //var fired = _regressionViewModel.IsPropertyChangedFired(() =>
            //{
            //    _regressionViewModel.RegressionDataViewModel = new RegressionDataViewModel(_regression);
            //}, nameof(_regressionViewModel.RegressionDataViewModel));

            //Assert.IsTrue(fired);
            Assert.Fail();
        }

        [TestMethod()]
        public void LoadTest()
        {
            Assert.Fail();
        }
    }
}