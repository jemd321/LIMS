using System;
using System.Collections.ObjectModel;
using System.Linq;
using LIMS.CustomEvent;
using LIMS.Model.RegressionModels;
using LIMS.Utility;

namespace LIMS.ViewModel
{
    /// <summary>
    /// ViewModel to represent standard and QC data as part of the regression.
    /// </summary>
    public class RegressionDataViewModel : ViewModelBase, IRegressionDataViewModel
    {
        private readonly Regression _currentRegression;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegressionDataViewModel"/> class.
        /// </summary>
        /// <param name="currentRegression">The currently loaded regression.</param>
        public RegressionDataViewModel(Regression currentRegression)
        {
            _currentRegression = currentRegression;
            ConstructDataRows();
        }

        /// <summary>
        /// Handles events that signal that the regression should be run again, and the child viewModels refreshed.
        /// </summary>
        public event EventHandler RegressionUpdated;

        /// <summary>
        /// Gets an Observable Collection of the standards exposed as viewModels.
        /// </summary>
        public ObservableCollection<RegressionDataItemViewModel> Standards { get; } = new();

        /// <summary>
        /// Gets an Observable Collection of the QCs exposed as viewModels.
        /// </summary>
        public ObservableCollection<RegressionDataItemViewModel> QualityControls { get; } = new();

        /// <summary>
        /// Unsubscribes from all events from the datagrid row viewModels that are being listened to. Call this method when closing a run.
        /// </summary>
        public void UnsubscribeFromEvents()
        {
            foreach (var viewModel in Standards)
            {
                viewModel.RegressionDataChanged -= OnRegressionDataChanged;
            }

            foreach (var viewModel in QualityControls)
            {
                viewModel.RegressionDataChanged -= OnRegressionDataChanged;
            }
        }

        /// <summary>
        /// Raise the event that signals to the regressionViewModel that the regression needs to be recalculated, and the child viewModels refreshed.
        /// </summary>
        protected virtual void RaiseRegressionUpdated()
        {
            RegressionUpdated?.Invoke(this, EventArgs.Empty);
        }

        private void ConstructDataRows()
        {
            foreach (var standard in _currentRegression.RegressionData.Standards)
            {
                var vm = new RegressionDataItemViewModel(
                        sampleNumber: standard.SampleNumber,
                        sampleName: standard.SampleName,
                        sampleType: standard.SampleType,
                        instrumentResponse: $"{standard.InstrumentResponse:F1}",
                        accuracy: $"{standard.Accuracy:P1}",
                        calculatedConcentration: SignificantFigures.Format(standard.CalculatedConcentration.GetValueOrDefault(), 3),
                        nominalConcentration: $"{standard.NominalConcentration:F0}",
                        isActive: standard.IsActive);
                vm.RegressionDataChanged += OnRegressionDataChanged;
                Standards.Add(vm);
            }

            foreach (var qualityControl in _currentRegression.RegressionData.QualityControls)
            {
                QualityControls.Add(
                    new RegressionDataItemViewModel(
                        sampleNumber: qualityControl.SampleNumber,
                        sampleName: qualityControl.SampleName,
                        sampleType: qualityControl.SampleType,
                        instrumentResponse: $"{qualityControl.InstrumentResponse:F1}",
                        accuracy: $"{qualityControl.Accuracy:P1}",
                        calculatedConcentration: SignificantFigures.Format(qualityControl.CalculatedConcentration.GetValueOrDefault(), 3),
                        nominalConcentration: $"{qualityControl.NominalConcentration:F0}",
                        isActive: qualityControl.IsActive));
            }
        }

        private void OnRegressionDataChanged(object sender, RegressionChangedEventArgs e)
        {
            int changedSampleNumber = e.SampleNumber;
            bool isActive = e.IsActive;
            _currentRegression.RegressionData.Standards.
                Select(standard => standard).
                Single(standard => standard.SampleNumber == changedSampleNumber).
                IsActive = isActive;
            RaiseRegressionUpdated();
        }
    }
}
