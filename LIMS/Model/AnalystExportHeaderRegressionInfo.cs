using LIMS.Enums;

namespace LIMS.Model
{
    /// <summary>
    /// The information from an Analyst software export describing the regression parameters caluclated in Analyst.
    /// </summary>
    public readonly record struct AnalystExportHeaderRegressionInfo
    {
        /// <summary>
        /// Gets the type of <see cref="Regression"/> used.
        /// </summary>
        public RegressionType Regression { get; init; }

        /// <summary>
        /// Gets the <see cref="WeightingFactor"/> applied to the regression.
        /// </summary>
        public WeightingFactor WeightingFactor { get; init; }

        /// <summary>
        /// Gets the gradient of the regression if linear, or the 'a' term of a polynomial regression equation.
        /// </summary>
        public double? A { get; init; }

        /// <summary>
        /// Gets the y-intercept of the regression if linear, or the 'b' term of a polynomial regression equation.
        /// </summary>
        public double? B { get; init; }

        /// <summary>
        /// Gets the 'c' term of a polynomial regression equation.
        /// </summary>
        public double? C { get; init; }

        /// <summary>
        /// Gets the coefficient of determination, R squared.
        /// </summary>
        public double RSquared { get; init; }
    }
}
