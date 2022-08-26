using LIMS.Enums;

namespace LIMS.Model
{
    /// <summary>
    /// <summary>
    /// Unified return object from parsing of imported data, including result and whether the operation succeeded.
    /// </summary>
    public class ImportedDataParseResult
    {
        /// <summary>
        /// Gets the data that has been parsed.
        /// </summary>
        public RegressionData Data { get; init; }

        /// <summary>
        /// Gets the reason that the parsing failed.
        /// </summary>
        public ParseFailureReason ParseFailureReason { get; init; }

        /// <summary>
        /// <c>true</c> if the parsing succeeded; otherwise, <c>false</c>.
        /// </summary>
        public bool IsSuccess => ParseFailureReason == ParseFailureReason.None;
    }
}
