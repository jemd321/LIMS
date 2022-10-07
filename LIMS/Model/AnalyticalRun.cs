using LIMS.Enums;

namespace LIMS.Model
{
    /// <summary>
    /// A record representing a uniquely named analytical run under a particular project, with a reference to the data.
    /// </summary>
    public record AnalyticalRun
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyticalRun"/> class.
        /// </summary>
        /// <param name="analyticalRunID">The unique ID of the analytical run.</param>
        /// <param name="parentProjectID">The ID of the project that this analytical run is filed under.</param>
        /// <param name="regressionData">The regression data held for this run.</param>
        /// <param name="regressionType">The regression type last selected for this run.</param>
        /// <param name="weightingFactor">The weighting factor last selected for this run.</param>
        public AnalyticalRun(
            string analyticalRunID,
            string parentProjectID,
            RegressionData regressionData,
            RegressionType regressionType = RegressionType.Linear,
            WeightingFactor weightingFactor = WeightingFactor.OneOverXSquared)
        {
            AnalyticalRunID = analyticalRunID;
            ParentProjectID = parentProjectID;
            RegressionData = regressionData;
            RegressionType = regressionType;
            WeightingFactor = weightingFactor;
        }

        /// <summary>
        /// Gets or sets the unique ID of this analytical run.
        /// </summary>
        public string AnalyticalRunID { get; set; }

        /// <summary>
        /// Gets the ID of the project that this analytical run is filed under.
        /// </summary>
        public string ParentProjectID { get; }

        /// <summary>
        /// Gets or sets the regression data held for this run.
        /// </summary>
        public RegressionData RegressionData { get; set; }

        /// <summary>
        /// Gets or sets the regression type last selected for this run.
        /// </summary>
        public RegressionType RegressionType { get; set; }

        /// <summary>
        /// Gets or sets the weighting factor last selected for this run.
        /// </summary>
        public WeightingFactor WeightingFactor { get; set; }
    }
}
