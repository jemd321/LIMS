using System.Collections.ObjectModel;
using LIMS.Model.RegressionModels;

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
                    new RegressionDataItemViewModel()
                    {
                        SampleName = standard.SampleName,
                        SampleType = standard.SampleType,
                        InstrumentResponse = standard.InstrumentResponse,
                        Accuracy = standard.Accuracy,
                        CalculatedConcentration = standard.CalculatedConcentration,
                        NominalConcentration = standard.NominalConcentration,
                        IsActive = standard.IsActive
                    });
            }

            foreach (var qualityControl in _currentRegression.RegressionData.QualityControls)
            {
                QualityControls.Add(
                    new RegressionDataItemViewModel()
                    {
                        SampleName = qualityControl.SampleName,
                        SampleType = qualityControl.SampleType,
                        InstrumentResponse = qualityControl.InstrumentResponse,
                        Accuracy = qualityControl.Accuracy,
                        CalculatedConcentration = qualityControl.CalculatedConcentration,
                        NominalConcentration = qualityControl.NominalConcentration,
                        IsActive = qualityControl.IsActive
                    });
            }
        }

        public ObservableCollection<RegressionDataItemViewModel> Standards { get; } = new();

        public ObservableCollection<RegressionDataItemViewModel> QualityControls { get; } = new();
    }
}
