using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Data
{
    public record AnalystExport
    {
        public List<AnalystExportHeaderPeakInfo> Peaks { get; init; }
        public AnalystExportHeaderRegressionInfo AnalystRegressionInfo { get; init; }
        public List<AnalystExportRow> DataRows { get; init; }

    }
}
