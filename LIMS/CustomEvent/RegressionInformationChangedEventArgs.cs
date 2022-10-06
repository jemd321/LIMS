using LIMS.Enums;

namespace LIMS.CustomEvent
{
    /// <summary>
    /// Event Args for the regression information changed event, describing the change in the regression type or weighting factor.
    /// </summary>
    public class RegressionInformationChangedEventArgs
    {
        /// <summary>
        /// Gets or sets the regression type that the user has selected to switch the regression to.
        /// </summary>
        public RegressionType RegressionType { get; set; }

        /// <summary>
        /// Gets or sets the Weighting factor that the user has selected to switch the regression to.
        /// </summary>
        public WeightingFactor WeightingFactor { get; set; }
    }
}