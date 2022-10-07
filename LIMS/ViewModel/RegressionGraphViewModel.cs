using System.Linq;
using LIMS.Model.RegressionModels;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace LIMS.ViewModel
{
    /// <summary>
    /// ViewModel for display of the calibration curve/regression graph.
    /// </summary>
    public class RegressionGraphViewModel : ViewModelBase, IRegressionGraphViewModel
    {
        private PlotModel _calibrationCurve;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegressionGraphViewModel"/> class.
        /// </summary>
        /// <param name="regression">The currently open regression to be graphed.</param>
        public RegressionGraphViewModel(Regression regression)
        {
            Regression = regression;
        }

        /// <summary>
        /// Gets or sets a reference to the regression contained within the RegressionViewModel.
        /// </summary>
        /// <remarks>Allows updates to the regression in the other regressionViewModels to propagate here for display.</remarks>
        public Regression Regression { get; set; }

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
            if (CalibrationCurve is null)
            {
                SetupGraphInitial();
                SetupAxesInitial();
            }

            CalibrationCurve.Series.Clear();
            DrawRegressionDataPoints();
            DrawRegressionEquationLine();

            // When any datapoint/regression information is updated the graph must be refreshed by calling the invalidate plot method.
            // When the view is first created the plotview is null, so no invalidation is required.
            if (CalibrationCurve.PlotView is not null)
            {
                bool updateRequired = true;
                CalibrationCurve.InvalidatePlot(updateRequired);
            }
        }

        private void SetupGraphInitial()
        {
            CalibrationCurve = new PlotModel
            { Title = "Calibration Curve" };
        }

        private void SetupAxesInitial()
        {
            // Determine highest concentration value for the x-axis (i.e. the highest standard in the assay)
            double highestStandardResponse = Regression.RegressionData.Standards.Select(s => s.InstrumentResponse).Max().GetValueOrDefault();
            double highestStandardConc = Regression.RegressionData.Standards.Select(s => s.NominalConcentration).Max().GetValueOrDefault();

            // Multiply by 1.3 to allow some extrapolation of the calibration of the line beyond the assay range.
            double yHigh = highestStandardResponse * 1.3;
            double xHigh = highestStandardConc * 1.3;

            var xAxis = new LinearAxis()
            {
                Position = AxisPosition.Bottom,
                Title = "Concentration (ng/mL)",

                // Only min and max values are set since the graph adjusts the axes automatically based on the dataset.
                AbsoluteMinimum = 0,
                AbsoluteMaximum = xHigh,
            };

            var yAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Instrument Response",
                AbsoluteMinimum = 0,
                AbsoluteMaximum = yHigh,
            };

            CalibrationCurve.Axes.Add(yAxis);
            CalibrationCurve.Axes.Add(xAxis);
        }

        private void DrawRegressionDataPoints()
        {
            // Create points to plot - nominal concentration on x axis, instrument response on y axis.
            const string BLUECOLORFORACTIVEMARKER = "#543db3";
            const string REDCOLORFORDEACTIVEDMARKER = "#f72020";
            var activatedPoints = new ScatterSeries
            {
                MarkerFill = OxyColor.Parse(BLUECOLORFORACTIVEMARKER),
                MarkerSize = 4,
                MarkerType = MarkerType.Diamond,
            };
            var deactivatedPoints = new ScatterSeries
            {
                MarkerFill = OxyColor.Parse(REDCOLORFORDEACTIVEDMARKER),
                MarkerSize = 4,
                MarkerType = MarkerType.Triangle,
            };

            foreach (Standard standard in Regression.RegressionData.Standards)
            {
                var x = standard.NominalConcentration.GetValueOrDefault();
                var y = standard.InstrumentResponse.GetValueOrDefault();

                if (!standard.IsActive)
                {
                    deactivatedPoints.Points.Add(new ScatterPoint(x, y));
                    continue;
                }

                activatedPoints.Points.Add(new ScatterPoint(x, y));
            }

            // Add points to display as a series.
            CalibrationCurve.Series.Add(activatedPoints);
            CalibrationCurve.Series.Add(deactivatedPoints);
        }

        private void DrawRegressionEquationLine()
        {
            // Get highest nominal concentration - ie. the assay range for x axis display. Lowest is set to 0 so the line
            // is extrapolated as far left as possible.
            double xLow = 0;
            double highestStandardConc = Regression.RegressionData.Standards.Select(s => s.NominalConcentration).Max().GetValueOrDefault();

            // Multiply by 1.3 to allow some extrapolation of the calibration of the line beyond the assay range.
            double xHigh = highestStandardConc * 1.3;

            // The number of points generated on the line (affecting smoothness of the curve)
            const double LINEPOINTINTEVRAL = 0.1;

            // Plot the equation of the linear regression as a second series to be displayed at the same time.
            var a = Regression.ATerm.GetValueOrDefault();
            var b = Regression.BTerm.GetValueOrDefault();
            double RegressionLineEquation(double x) => (a * x) + b;

            const string GREENCOLORFORLINE = "#0fba48";
            var calibrationLine = new FunctionSeries(RegressionLineEquation, xLow, xHigh, LINEPOINTINTEVRAL)
            {
                Color = OxyColor.Parse(GREENCOLORFORLINE),
            };

            CalibrationCurve.Series.Add(calibrationLine);
        }
    }
}
