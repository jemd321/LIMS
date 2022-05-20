using System;
using System.Collections.Generic;
using System.IO;

namespace LIMS.Model
{
    public class AnalystExportParser
    {
        public enum AnalystExportSections
        {
            PeakInfo,
            RegressionInfo,
            DataRows
        }
        public AnalystExportParser(string filePath)
        {
            using var reader = new StreamReader(filePath);

            var currentSection = AnalystExportSections.PeakInfo;
            var buffer = new List<string>();

            while (!reader.EndOfStream)
            {
                if (currentSection == AnalystExportSections.DataRows)
                {
                    ProcessDataRow();
                }

                string line = reader.ReadLine();
                // peak obj
                // regression info
                // line
            }

        }

        private void ProcessDataRow()
        {
            throw new NotImplementedException();
        }
    }
}
