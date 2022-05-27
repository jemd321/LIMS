using LIMS.Data;
using LIMS.Model;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace LIMS.ViewModel
{
    public class RegressionViewModel : ViewModelBase
    {
        private AnalystExport _analystExport;

        private readonly IRegressionDataProvider _regressionDataProvider;

        public RegressionViewModel(IRegressionDataProvider regressionDataProvider)
        {
            _regressionDataProvider = regressionDataProvider;
        }

        public AnalystExport AnalystExport
        {
            get => _analystExport;
            set
            {
                _analystExport = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<AnalystExportRow> DataGridSource { get; } = new();

        public async override Task Load()
        {
            AnalystExport = await _regressionDataProvider.GetData();
            foreach (var datarow in AnalystExport.DataRows)
            {
                DataGridSource.Add(datarow);
            }

    
        }


    }
}
