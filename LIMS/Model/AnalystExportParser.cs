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

        private static AnalystExportHeaderPeakInfo ProcessPeakInfo(List<string> headerBuffer)
        {
            string peakName = headerBuffer[0].Split(' ')[1];
            bool isInternalStandard = headerBuffer[1].TrimEnd() == "Use as Internal Standard";
            string internalStandard = null;
            if (!isInternalStandard)
            {
                internalStandard = headerBuffer[1].Split(' ')[1];
            }
            var transitionMRM = ParseTransition(headerBuffer[2]);
            return new AnalystExportHeaderPeakInfo()
            {
                PeakName = peakName,
                IsInternalStandard = isInternalStandard,
                InternalStandard = internalStandard,
                TransitionMRM = transitionMRM
            };
        }

        private static TransitionMRM ParseTransition(string line)
        {
            string transition = line.Split(' ')[1];
            var splitTransition = transition.Split('/');
            double Q1 = double.Parse(splitTransition[0]);
            double Q3 = double.Parse(splitTransition[1]);
            return new TransitionMRM() { Q1 = Q1, Q3 = Q3 };
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
