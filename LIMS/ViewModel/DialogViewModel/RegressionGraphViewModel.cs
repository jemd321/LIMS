using System.Linq;
using LIMS.Model.RegressionModels;
using OxyPlot;
using OxyPlot.Series;

namespace LIMS.ViewModel
{
    /// <summary>
    /// ViewModel for display of the calibration curve/regression graph.
    /// </summary>
    public class RegressionGraphViewModel : ViewModelBase, IRegressionGraphViewModel
    {
        private readonly Regression _regression;
        private PlotModel _calibrationCurve;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegressionGraphViewModel"/> class.
        /// </summary>
        /// <param name="regression">The currently open regression to be graphed.</param>
        public RegressionGraphViewModel(Regression regression)
        {
            _regression = regression;
        }

        /// <summary>
        /// Gets the plot that the plotView control binds to.
        /// </summary>
        public PlotModel CalibrationCurve
        {
            get => _calibrationCurve;
            private set
            {
                _calibrationCurve = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Loads the data from the current regression into the plot for display. Updates an existing graph when called again.
        /// </summary>
        public void DrawGraph()
        {
            CalibrationCurve = new PlotModel { Title = "Calibration Curve" };

            // Create points to plot - nominal concentration on x axis, instrument response on y axis.
            var scatterSeries = new ScatterSeries();
            foreach (Standard standard in _regression.RegressionData.Standards)
            {
                var x = standard.NominalConcentration.GetValueOrDefault();
                var y = standard.InstrumentResponse.GetValueOrDefault();
                scatterSeries.Points.Add(new ScatterPoint(x, y));
            }

            // Add points to display as a series.
            CalibrationCurve.Series.Add(scatterSeries);

            // Get lowest and highest nominal concentration - ie. the assay range for x axis display.
            var xLow = _regression.RegressionData.Standards.Select(s => s.NominalConcentration).Min().GetValueOrDefault();
            var xHigh = _regression.RegressionData.Standards.Select(s => s.NominalConcentration).Max().GetValueOrDefault();
            const double LINEPOINTINTEVRAL = 0.1;

            // Plot the equation of the linear regression as a second series to be displayed at the same time.
            var m = _regression.Gradient.GetValueOrDefault();
            var c = _regression.YIntercept.GetValueOrDefault();

            double RegressionLineEquation(double x) => (m * x) + c;
            var calibrationLine = new FunctionSeries(RegressionLineEquation, xLow, xHigh, LINEPOINTINTEVRAL);
            CalibrationCurve.Series.Add(calibrationLine);
        }
    }
}
