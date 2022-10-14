using LIMS.Model.RegressionModels;
using OxyPlot;

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

        /// <summary>
        /// Gets or sets the plot that the plotView control binds to.
        /// </summary>
        public PlotModel CalibrationCurve { get; set; }

        /// <summary>
        /// Renders the graph, based on the data that has been supplied previously.
        /// </summary>
        public void DrawGraph();
    }
}