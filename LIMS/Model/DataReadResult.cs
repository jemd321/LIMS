using LIMS.Enums;

namespace LIMS.Model
{
    /// <summary>
    /// Unified return object from data reading operations, including result and whether the operation succeeded.
    /// </summary>
    public record DataReadResult
    {
        /// <summary>
        /// Gets the data that has been read.
        /// </summary>
        public string Data { get; init; }

        /// <summary>
        /// Gets the reason that the read operation failed.
        /// </summary>
        public DataReadFailureReason DataReadFailureReason { get; init; }

        /// <summary>
        /// <c>true</c> if the data read operation succeeded; otherwise, <c>false</c>.
        /// </summary>
        public bool IsSuccess => DataReadFailureReason == DataReadFailureReason.None;
    }
}