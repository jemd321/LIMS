using System;
using System.Collections.Generic;
using System.IO;

namespace LIMS.Model
{
    public static class AnalystExportParser
    {
        public enum AnalystExportSections
        {
            PeakInfo,
            RegressionInfo,
            DataRows
        }

        public static void ReadAnalystExport(string filePath)
        {
            using var reader = new StreamReader(filePath);

            var currentSection = AnalystExportSections.PeakInfo;
            var buffer = new List<string>();

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (currentSection == AnalystExportSections.DataRows)
                {
                    ProcessDataRow();
                    continue;
                }
                if (line == @"\r\n")
                {

                }
                // peak obj
                // regression info
                // line
            }
        }

        private static void ProcessDataRow()
        {
            throw new NotImplementedException();
        }
    }
}
