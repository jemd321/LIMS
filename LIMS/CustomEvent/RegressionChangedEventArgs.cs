using System;

namespace LIMS.CustomEvent
{
    /// <summary>
    /// Custom EventArgs for transmitting which updates have been made to a regression in the UI.
    /// </summary>
    public class RegressionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the sequence number of the sample - ie. the position in the order that the run was acquired.
        /// </summary>
        public int SampleNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the sample should be included in the regression and/or statistical calculations.
        /// </summary>
        public bool IsActive { get; set; }
    }
}
