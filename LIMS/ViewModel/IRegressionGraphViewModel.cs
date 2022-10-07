using LIMS.Model.RegressionModels;

namespace LIMS.ViewModel
{
    /// <summary>
    /// ViewModel for display of the calibration curve/regression graph.
    /// </summary>
    public interface IRegressionGraphViewModel
    {
        /// <summary>
        /// Gets or sets a reference to the regression contained within the RegressionViewModel.
        /// </summary>
        /// <remarks>Allows updates to the regression in the other regressionViewModels to propagate here for display.</remarks>
        public Regression Regression { get; set; }

        public void DrawGraph();
    }
}