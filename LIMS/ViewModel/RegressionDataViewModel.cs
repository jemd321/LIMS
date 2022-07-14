using LIMS.Model.RegressionModels;
using System.Collections.ObjectModel;

namespace LIMS.ViewModel
{
    public class RegressionDataViewModel : ViewModelBase
    {
        private readonly LinearRegression _currentRegression;

        public RegressionDataViewModel(LinearRegression currentRegression)
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
                    }) ;
            }
        }

        public ObservableCollection<RegressionDataItemViewModel> Standards { get; } = new();



    }
}
