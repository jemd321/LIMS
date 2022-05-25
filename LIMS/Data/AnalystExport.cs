using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Data
{
    public readonly record struct AnalystExport
    {
        public readonly List<AnalystExportHeaderPeakInfo> Peaks { get; init; }
        public readonly AnalystExportHeaderRegressionInfo AnalystRegressionInfo { get; init; }
        public readonly List<AnalystExportRow> DataRows { get; init; }

    }
}
