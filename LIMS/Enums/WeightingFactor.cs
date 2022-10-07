namespace LIMS.Enums
{
    /// <summary>
    /// The weighing factor selected to account for the heteroscedascticity of the instrument responses used in the regression.
    /// </summary>
    public enum WeightingFactor
    {
        /// <summary>
        /// No weighting factor applied to the regression.
        /// </summary>
        Unweighted,

        /// <summary>
        /// Weighting factor of 1/x^1/2 applied to the regression.
        /// </summary>
        OneOverXHalf,

        /// <summary>
        /// Weighting factor of 1/x applied to the regression.
        /// </summary>
        OneOverX,

        /// <summary>
        /// Weighting factor of 1/x^2 applied to the regression.
        /// </summary>
        OneOverXSquared,

        /// <summary>
        /// Weighting factor of 1/y^1/2 applied to the regression.
        /// </summary>
        OneOverYHalf,

        /// <summary>
        /// Weighting factor of 1/y applied to the regression.
        /// </summary>
        OneOverY,

        /// <summary>
        /// Weighting factor of 1/y^2 applied to the regression.
        /// </summary>
        OneOverYSquared,
    }
}
