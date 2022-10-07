using LIMS.CustomEvent;
using LIMS.Enums;
using LIMS.Factory;
using LIMS.Model;
using LIMS.Model.RegressionModels;

namespace LIMS.ViewModel
{
    /// <summary>
    /// Top level viewModel to handle interaction with a regression.
    /// </summary>
    public class RegressionViewModel : ViewModelBase, IRegressionViewModel
    {
        private readonly IRegressionFactory _regressionFactory;
        private IRegressionDataViewModel _regressionDataViewModel;
        private IRegressionGraphViewModel _regressionGraphViewModel;
        private IRegressionInformationViewModel _regressionInformationViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegressionViewModel"/> class.
        /// </summary>
        /// <param name="regressionFactory">factory to build a regression object based on the type required.</param>
        public RegressionViewModel(IRegressionFactory regressionFactory)
        {
            _regressionFactory = regressionFactory;
        }

        /// <summary>
        /// Gets or sets the viewModel that controls the regression 'data' view.
        /// </summary>
        public IRegressionDataViewModel RegressionDataViewModel
        {
            get => _regressionDataViewModel;
            set
            {
                _regressionDataViewModel = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the viewModel that controls the regression 'graph' view.
        /// </summary>
        public IRegressionGraphViewModel RegressionGraphViewModel
        {
            get => _regressionGraphViewModel;
            set
            {
                _regressionGraphViewModel = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the viewModel that controls the regression 'information' view.
        /// </summary>
        public IRegressionInformationViewModel RegressionInformationViewModel
        {
            get => _regressionInformationViewModel;
            set
            {
                _regressionInformationViewModel = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the currently open analytical run.
        /// </summary>
        public AnalyticalRun OpenAnalyticalRun { get; set; }

        /// <summary>
        /// Gets or sets the currently loaded regression.
        /// </summary>
        public Regression Regression { get; set; }

        /// <summary>
        /// Load method to initialise the viewModel.
        /// </summary>
        /// <param name="analyticalRun">The analytical run supplied to the viewModel, containing the data required.</param>
        public override void Load(AnalyticalRun analyticalRun)
        {
            OpenAnalyticalRun = analyticalRun;
            Regression = _regressionFactory.ConstructRegression(
                analyticalRun.RegressionData,
                analyticalRun.RegressionType,
                analyticalRun.WeightingFactor);
            LoadChildViewModels();
        }

        private void LoadChildViewModels()
        {
            UnsubscribeFromEvents();

            RegressionType regressionType = Regression.RegressionType;
            WeightingFactor weightingFactor = Regression.WeightingFactor;
            double gradient = Regression.ATerm.GetValueOrDefault();
            double yIntercept = Regression.BTerm.GetValueOrDefault();

            RegressionDataViewModel = new RegressionDataViewModel(Regression);
            RegressionDataViewModel.RegressionUpdated += OnRegressionUpdated;

            RegressionInformationViewModel = new RegressionInformationViewModel(regressionType, weightingFactor, gradient, yIntercept);
            RegressionInformationViewModel.RegressionInformationChanged += OnRegressionInformationChanged;

            // Testing
            if (RegressionGraphViewModel is null)
            {
                RegressionGraphViewModel = new RegressionGraphViewModel(Regression);
            }

            RegressionGraphViewModel.Regression = Regression;
            RegressionGraphViewModel.DrawGraph();
        }

        private void UnsubscribeFromEvents()
        {
            // Unsubscribe from events to prevent memory leaks when updating the regression.
            if (RegressionDataViewModel is not null)
            {
                RegressionDataViewModel.RegressionUpdated -= OnRegressionUpdated;
            }

            if (RegressionInformationViewModel is not null)
            {
                RegressionInformationViewModel.RegressionInformationChanged -= OnRegressionInformationChanged;
            }
        }

        private void OnRegressionUpdated(object sender, System.EventArgs e)
        {
            Regression.RunRegression();
            OpenAnalyticalRun.RegressionData = Regression.RegressionData;
            OpenAnalyticalRun.RegressionType = Regression.RegressionType;
            OpenAnalyticalRun.WeightingFactor = Regression.WeightingFactor;
            LoadChildViewModels();
        }

        private void OnRegressionInformationChanged(object sender, RegressionInformationChangedEventArgs e)
        {
            var userSelectedRegressionType = e.RegressionType;
            var userSelectedWeightingFactor = e.WeightingFactor;
            Regression = _regressionFactory.ConstructRegression(Regression.RegressionData, userSelectedRegressionType, userSelectedWeightingFactor);
            OpenAnalyticalRun.RegressionData = Regression.RegressionData;
            OpenAnalyticalRun.RegressionType = Regression.RegressionType;
            OpenAnalyticalRun.WeightingFactor = Regression.WeightingFactor;
            LoadChildViewModels();
        }
    }
}
