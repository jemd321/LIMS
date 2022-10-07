using LIMS.Enums;

namespace LIMS.Model.RegressionModels
{
    /// <summary>
    /// A base class representing a regression, allowing the calculation of the line equation, standard & QC accuracy and unknown concentration.
    /// </summary>
    public abstract class Regression
    {
        /// <summary>
        /// Gets the data that will be used in the regression.
        /// </summary>
        public RegressionData RegressionData { get; private protected set; }

        /// <summary>
        /// Gets the type of the regression that is currently used.
        /// </summary>
        public RegressionType RegressionType { get; private protected set; }

        /// <summary>
        /// Gets the weighting factor that is applied to the regression.
        /// </summary>
        public WeightingFactor WeightingFactor { get; private protected set; }

        /// <summary>
        /// Gets the A term of the regression.
        /// </summary>
        /// <remarks>For a linear regression this is the gradient.</remarks>
        public double? ATerm { get; private protected set; }

        /// <summary>
        /// Gets the B term of the regression.
        /// </summary>
        /// <remarks>For a linear regression this is the y-intercept.</remarks>
        public double? BTerm { get; private protected set; }

        /// <summary>
        /// Calculates all of the parameters of the regression, including unknown sample concentrations.
        /// </summary>
        public abstract void RunRegression();
    }
}