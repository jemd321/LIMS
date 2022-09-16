using LIMS.Enums;

namespace LIMS.Model.RegressionModels
{
    /// <summary>
    /// A class representing the shared properties needed for each sample to be regressed.
    /// </summary>
    public abstract class RegressionDataPoint
    {
        /// <summary>
        /// Gets the sequence number of the sample - ie. the position in the order that the run was acquired.
        /// </summary>
        public int SampleNumber { get; init; }

        /// <summary>
        /// Gets the name of the sample.
        /// </summary>
        public string SampleName { get; init; }

        /// <summary>
        /// Gets the category of the sample.
        /// </summary>
        public abstract SampleType SampleType { get; }

        /// <summary>
        /// Gets the instrument response of the sample - ie. the peak area.
        /// </summary>
        public double? InstrumentResponse { get; init; }

        /// <summary>
        /// Gets or sets the concentration calculated by the regression.
        /// </summary>
        public double? CalculatedConcentration { get; set; }

        /// <summary>
        /// Gets or sets the known concentration that the sample was prepared at.
        /// </summary>
        public double? NominalConcentration { get; set; }

        /// <summary>
        /// Gets or sets the bias of the sample relative to the nominal concentration in percent.
        /// </summary>
        public double? Accuracy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the sample should be included in the regression and/or statistical calculations.
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
