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

        const string NEWLINE = @"\r\n";
        public static void ReadAnalystExport(string filePath)
        {
            
            
            using var reader = new StreamReader(filePath);

            var currentSection = AnalystExportSections.PeakInfo;
            var buffer = new List<string>();

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (line == NEWLINE)
                {
                    if (IsBlockRegressionInfo(buffer))
                    {

                    }
                }

                if (currentSection == AnalystExportSections.DataRows)
                {
                    ProcessDataRow();
                    continue;
                }

                // peak obj
                // regression info
                // line
            }
        }

        private static bool IsBlockRegressionInfo(List<string> buffer)
        {
            throw new NotImplementedException();
        }

        private static void ProcessDataRow()
        {
            throw new NotImplementedException();
        }
    }
}
