namespace LIMS.Model
{
    /// <summary>
    /// A record struct representing a mass spectrometry transition in multiple reaction monitoring mode (MRM). Both values are mass/charge ratios in Daltons.
    /// </summary>
    public readonly record struct TransitionMRM
    {
        /// <summary>
        /// Gets the mass of the precursor ion in Daltons.
        /// </summary>
        public double Q1 { get; init; }

        /// <summary>
        /// Gets the mass of the product ion in Daltons.
        /// </summary>
        public double Q3 { get; init; }
    }
}
