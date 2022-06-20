using LIMS.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LIMS.ViewModel
{
    public class RegressionDataViewModel : ViewModelBase
    {
        private readonly AnalystExport _analystExport;

        public RegressionDataViewModel(AnalystExport analystExport)
        {
            _analystExport = analystExport;
            foreach (var row in _analystExport.DataRows)
            {
                DataRows.Add(new RegressionDataItemViewModel()
                {
                    SampleNumber = row.SetNumber,
                    SampleName = row.SampleName,
                    SampleType = row.SampleType,
                    InstrumentResponse = row.ResponseFactor1,
                    Bias = 100d,
                    IncludeInRegression = row.UseRecord
                });
            }
        }

        public ObservableCollection<RegressionDataItemViewModel> DataRows { get; } = new();



    }
}
