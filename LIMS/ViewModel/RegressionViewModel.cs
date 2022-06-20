using LIMS.Model;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace LIMS.ViewModel
{
    public class RegressionViewModel : ViewModelBase
    {

        private readonly IRegressionDataProvider _regressionDataProvider;

        public RegressionViewModel(IRegressionDataProvider regressionDataProvider)
        {
            _regressionDataProvider = regressionDataProvider;
        }

        public RegressionDataViewModel RegressionDataViewModel { get; private set; }
        public RegressionStatisticsViewModel RegressionStatisticsViewModel { get; private set; }
        public RegressionGraphViewModel RegressionGraphViewModel { get; private set; }
        public RegressionISPlotViewModel RegressionISPlotViewModel { get; private set; }

        public async override Task Load(string rawData)
        {
            RegressionData regressionData = await _regressionDataProvider.GetRegressionData(rawData);
        }


    }
}
