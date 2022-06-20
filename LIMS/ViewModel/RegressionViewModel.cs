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

        public RegressionDataViewModel RegressionDataViewModel { get; private set; }
        public RegressionStatisticsViewModel RegressionStatisticsViewModel { get; private set; }
        public RegressionGraphViewModel RegressionGraphViewModel { get; private set; }
        public RegressionISPlotViewModel RegressionISPlotViewModel { get; private set; }

        public AnalystExport AnalystExport
        {
            get => _analystExport;
            set
            {
                _analystExport = value;
                RaisePropertyChanged();
            }
        }

        public async override Task Load()
        {
            AnalystExport = await _regressionDataProvider.GetData();
            RegressionDataViewModel = new RegressionDataViewModel(AnalystExport);

    
        }


    }
}
