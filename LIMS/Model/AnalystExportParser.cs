using LIMS.Data;
using LIMS.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace LIMS.Model
{
    public static class AnalystExportParser
    {
        const string NEWLINE = @"\r\n";
        public static void ReadAnalystExport(string filePath)
        {
            var headerPeakInfo = new List<AnalystExportHeaderPeakInfo>();
            var headerRegressionInfo = new List<AnalystExportHeaderRegressionInfo>();
            var dataRows = new List<AnalystExportRow>();

            using var reader = new StreamReader(filePath);

            var currentSection = AnalystExportSections.Header;
            var headerBuffer = new List<string>();

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (line == NEWLINE)
                {
                    switch (currentSection)
                    {
                        case AnalystExportSections.Header:
                            if (BlockIsRegressionInfo(headerBuffer))
                            {
                                ProcessRegressionInfo(headerBuffer);
                                currentSection = AnalystExportSections.DataRows;
                            }
                            else
                            {
                                ProcessPeakInfo(headerBuffer);
                            }
                            break;
                        case AnalystExportSections.DataRows:
                            break;
                    }
                }
                if (currentSection == AnalystExportSections.DataRows)
                {
                    ProcessDataRow(line);
                    continue;
                }
                headerBuffer.Add(line);
            }
        }

        private static void ProcessPeakInfo(List<string> headerBuffer)
        {
            throw new NotImplementedException();
        }

        private static void ProcessRegressionInfo(List<string> headerBuffer)
        {
            throw new NotImplementedException();
        }

        private static bool BlockIsRegressionInfo(List<string> buffer)
        {
            throw new NotImplementedException();
        }

        private static void ProcessDataRow(string line)
        {
            throw new NotImplementedException();
        }
    }
}
