using LIMS.Model;
using LIMS.Model.RegressionModels;

namespace LIMS.ViewModel
{
    /// <summary>
    /// Interface for the viewModel that hosts a regression from a loaded dataset.
    /// </summary>
    public interface IRegressionViewModel
    {
        /// <summary>
        /// Gets or sets the viewModel that controls the regression 'data' view, displaying the standard, QC or unknown regression data.
        /// </summary>
        IRegressionDataViewModel RegressionDataViewModel { get; set; }

        /// <summary>
        /// Gets or sets the viewModel that controls the regression 'graph' view, displaying the calibration curve.
        /// </summary>
        IRegressionGraphViewModel RegressionGraphViewModel { get; set; }

        /// <summary>
        /// Gets or sets the viewModel that controls the regression 'information' view, displaying regression equation and weighting factor.
        /// </summary>
        IRegressionInformationViewModel RegressionInformationViewModel { get; set; }

        /// <summary>
        /// Gets or sets the currently open analytical run.
        /// </summary>
        AnalyticalRun OpenAnalyticalRun { get; set; }

        /// <summary>
        /// Gets or sets the currently loaded regression.
        /// </summary>
        Regression Regression { get; set; }
    }
}
