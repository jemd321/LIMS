using System.Collections.Generic;

namespace LIMS.Model
{
    /// <summary>
    /// The partially parsed data from an Analyst software export, which has been categorised into sections for further processing.
    /// </summary>
    public record AnalystExport
    {
        /// <summary>
        /// Gets the header sections of the parsed file which describe the analytes monitored in the run.
        /// </summary>
        public List<AnalystExportHeaderPeakInfo> Peaks { get; init; }

        /// <summary>
        /// Gets the header sections of the parsed file which describe the regression parameters used within Analyst (optional use).
        /// </summary>
        public AnalystExportHeaderRegressionInfo RegressionInfo { get; init; }

        /// <summary>
        /// Gets the data rows of the parsed file, containing the raw data of each sample acquired in the run.
        /// </summary>
        public List<AnalystExportRow> DataRows { get; init; }
    }
}
