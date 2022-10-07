using System.Collections.Generic;
using LIMS.Enums;
using LIMS.Model.RegressionModels;

namespace LIMS.Model
{
    /// <summary>
    /// A class representing the data that will be used to construct a regression, and the previously saved regression type and weighting factor if applicable.
    /// </summary>
    public class RegressionData
    {
        /// <summary>
        /// Gets a list of calibration standards used to in the calibration curve.
        /// </summary>
        public List<Standard> Standards { get; init; }

        /// <summary>
        /// Gets a list of quality control samples used to measure the performance of the assay.
        /// </summary>
        public List<QualityControl> QualityControls { get; init; }

        /// <summary>
        /// Gets a list of unknown samples, including blank or test samples.
        /// </summary>
        public List<Unknown> Unknowns { get; init; }
    }
}
