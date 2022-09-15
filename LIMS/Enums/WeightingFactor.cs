namespace LIMS.Enums
{
    /// <summary>
    /// The weighing factor selected to account for the heteroscedascticity of the instrument responses used in the regression.
    /// </summary>
    public enum WeightingFactor
    {
        Unweighted,
        OneOverXHalf,
        OneOverX,
        OneOverXSquared,
        OneOverYHalf,
        OneOverY,
        OneOverYSquared,
    }
}
