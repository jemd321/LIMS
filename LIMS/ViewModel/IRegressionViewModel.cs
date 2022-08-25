using LIMS.Model;
using LIMS.Model.RegressionModels;

namespace LIMS.ViewModel
{
    public interface IRegressionViewModel
    {
        IRegressionDataViewModel RegressionDataViewModel { get; }

        AnalyticalRun OpenAnalyticalRun { get; }

        Regression Regression { get; }
    }
}
