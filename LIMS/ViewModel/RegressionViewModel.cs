using LIMS.Factory;
using LIMS.Model;
using LIMS.Model.RegressionModels;

namespace LIMS.ViewModel
{
    public class RegressionViewModel : ViewModelBase, IRegressionViewModel
    {
        private readonly IRegressionFactory _regressionFactory;
        private IRegressionDataViewModel _regressionDataViewModel;

        public RegressionViewModel(IRegressionFactory regressionFactory)
        {
            _regressionFactory = regressionFactory;
        }

        public IRegressionDataViewModel RegressionDataViewModel
        {
            get => _regressionDataViewModel;
            set
            {
                _regressionDataViewModel = value;
                RaisePropertyChanged();
            }
        }

        public AnalyticalRun OpenAnalyticalRun { get; set; }

        public Regression Regression { get; set; }

        public RegressionStatisticsViewModel RegressionStatisticsViewModel { get; private set; }

        public override void Load(AnalyticalRun analyticalRun)
        {
            OpenAnalyticalRun = analyticalRun;
            Regression = _regressionFactory.ConstructRegression(
                analyticalRun.RegressionData,
                analyticalRun.RegressionType,
                analyticalRun.WeightingFactor);
            RegressionDataViewModel = new RegressionDataViewModel(Regression);
        }
    }
}
