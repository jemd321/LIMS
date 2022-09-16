using System.Collections.ObjectModel;
using LIMS.Model.RegressionModels;
using LIMS.Utility;

namespace LIMS.ViewModel
{
    public interface IRegressionDataViewModel
    {
        public ObservableCollection<RegressionDataItemViewModel> Standards { get; }

        public ObservableCollection<RegressionDataItemViewModel> QualityControls { get; }
    }

    public class RegressionDataViewModel : ViewModelBase, IRegressionDataViewModel
    {
        private readonly Regression _currentRegression;

        public RegressionDataViewModel(Regression currentRegression)
        {
            _currentRegression = currentRegression;
            ConstructDataRows();
        }

        private void ConstructDataRows()
        {
            foreach (var standard in _currentRegression.RegressionData.Standards)
            {
                Standards.Add(
                    new RegressionDataItemViewModel(
                        // SAMPLE NUMBER PLACEHOLDER VALUE
                        sampleNumber: 0,
                        sampleName: standard.SampleName,
                        sampleType: standard.SampleType,
                        instrumentResponse: $"{standard.InstrumentResponse:F1}",
                        accuracy: $"{standard.Accuracy:P1}",
                        calculatedConcentration: SignificantFigures.Format(standard.CalculatedConcentration.GetValueOrDefault(), 3),
                        nominalConcentration: $"{standard.NominalConcentration:F0}",
                        isActive: standard.IsActive));
            }

            foreach (var qualityControl in _currentRegression.RegressionData.QualityControls)
            {
                QualityControls.Add(
                    new RegressionDataItemViewModel(
                        // SAMPLE NUMBER PLACEHOLDER VALUE
                        sampleNumber: 0,
                        sampleName: qualityControl.SampleName,
                        sampleType: qualityControl.SampleType,
                        instrumentResponse: $"{qualityControl.InstrumentResponse:F1}",
                        accuracy: $"{qualityControl.Accuracy:P1}",
                        calculatedConcentration: SignificantFigures.Format(qualityControl.CalculatedConcentration.GetValueOrDefault(), 3),
                        nominalConcentration: $"{qualityControl.NominalConcentration:F0}",
                        isActive: qualityControl.IsActive));
            }
        }

        public ObservableCollection<RegressionDataItemViewModel> Standards { get; } = new();

        public ObservableCollection<RegressionDataItemViewModel> QualityControls { get; } = new();
    }
}
