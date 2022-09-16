using LIMS.Factory;
using LIMS.Model;
using LIMS.Model.RegressionModels;

namespace LIMS.ViewModel
{
    public class RegressionViewModel : ViewModelBase, IRegressionViewModel
    {
        private readonly IRegressionFactory _regressionFactory;
        private IRegressionDataViewModel _regressionDataViewModel;
        private IRegressionGraphViewModel _regressionGraphViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegressionViewModel"/> class.
        /// </summary>
        /// <param name="regressionFactory"></param>
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
        public IRegressionGraphViewModel RegressionGraphViewModel
        {
            get => _regressionGraphViewModel;
            set
            {
                _regressionGraphViewModel = value;
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
            RegressionGraphViewModel = new RegressionGraphViewModel(Regression);
            RegressionGraphViewModel.DrawGraph();
        }
    }
}
