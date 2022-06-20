using System.Collections.Generic;

namespace LIMS.Model
{
    public record AnalystExport
    {
        public List<AnalystExportHeaderPeakInfo> Peaks { get; init; }
        public AnalystExportHeaderRegressionInfo RegressionInfo { get; init; }
        public List<AnalystExportRow> DataRows { get; init; }

    }
}
