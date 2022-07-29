using LIMS.Factory;
using LIMS.Model;
using LIMS.Model.RegressionModels;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace LIMS.ViewModel
{
    public class RegressionViewModel : ViewModelBase
    {

        private readonly IRegressionDataProvider _regressionDataProvider;
        private readonly IRegressionFactory _regressionFactory;
        private RegressionDataViewModel _regressionDataViewModel;

        public RegressionViewModel(IRegressionDataProvider regressionDataProvider, IRegressionFactory regressionFactory)
        {
            _regressionDataProvider = regressionDataProvider;
            _regressionFactory = regressionFactory;
        }

        public RegressionDataViewModel RegressionDataViewModel
        {
            get => _regressionDataViewModel;
            set
            {  _regressionDataViewModel = value;
                RaisePropertyChanged();
            }
        }

        public RegressionStatisticsViewModel RegressionStatisticsViewModel { get; private set; }
        public RegressionGraphViewModel RegressionGraphViewModel { get; private set; }
        public RegressionISPlotViewModel RegressionISPlotViewModel { get; private set; }
        public Regression Regression { get; private set; }

        public async override Task Load(string rawData)
        {
            RegressionData regressionData = await _regressionDataProvider.GetRegressionData(rawData);
            Regression = _regressionFactory.ConstructRegression(regressionData);

            RegressionDataViewModel = new RegressionDataViewModel(Regression);
        }
    }
}
