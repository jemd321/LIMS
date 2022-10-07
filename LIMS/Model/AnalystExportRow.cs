using System;
using LIMS.Enums;

namespace LIMS.Model
{
    /// <summary>
    /// Record that represents a single row in an Analyst export file.
    /// </summary>
    public record AnalystExportRow
    {
#pragma warning disable SA1600 // Elements should be documented
        public string SampleName { get; init; }

        public int SampleID { get; init; }

        public SampleType SampleType { get; init; }

        public string SampleDescription { get; init; }

        public int SetNumber { get; init; }

        public string AcquisitonMethod { get; init; }

        public DateTime AcquisitionDate { get; init; }

        public string RackType { get; init; }

        public int RackNumber { get; init; }

        public int VialPosition { get; init; }

        public string PlateType { get; init; }

        public int PlateNumber { get; init; }

        public string FileName { get; init; }

        public double DilutionFactor { get; init; }

        public double WeightToVolumeRatio { get; init; }

        public string SampleAnnotation1 { get; init; }

        public string SampleAnnotation2 { get; init; }

        public string PeakName { get; init; }

        public Units Units { get; init; }

        public double Area { get; init; }

        public double Height { get; init; }

        public string AnalyteAnnotation { get; init; }

        public double NominalConcentration { get; init; }

        public double RetentionTime { get; init; }

        public double ExpectedRetentionTime { get; init; }

        public double RetentionTimeWindow { get; init; }

        public double CentroidLocation { get; init; }

        public int StartScan { get; init; }

        public double StartTime { get; init; }

        public int EndScan { get; init; }

        public double EndTime { get; init; }

        public string IntegrationType { get; init; }

        public double? SignalToNoiseRatio { get; init; }

        public double PeakWidth { get; init; }

        public string StandardQueryStatus { get; init; }

        public TransitionMRM AnalyteTransitionMRM { get; init; }

        public double? AnalyteToISAreaRatio { get; init; }

        public double? AnalyteToISHeightRatio { get; init; }

        public double AnalytePeakWidthAtHalfHeight { get; init; }

        public double AnalyteSlopeOfBaseline { get; init; }

        public string AnalyteProcessingAlgorithm { get; init; }

        public double AnalytePeakAsymmetry { get; init; }

        public string ISPeakName { get; init; }

        public Units ISUnits { get; init; }

        public double ISArea { get; init; }

        public double ISHeight { get; init; }

        public double ISConcentration { get; init; }

        public double ISRetentionTime { get; init; }

        public double ISExpectedRetentionTime { get; init; }

        public double ISRetentionTimeWindow { get; init; }

        public double ISCentroidLocation { get; init; }

        public int ISStartScan { get; init; }

        public double ISStartTime { get; init; }

        public int ISStopScan { get; init; }

        public double ISEndTime { get; init; }

        public string ISIntegrationType { get; init; }

        public double ISSignalToNoiseRatio { get; init; }

        public double ISPeakWidth { get; init; }

        public TransitionMRM ISTransitionMRM { get; init; }

        public double ISPeakWidthAtHalfHeight { get; init; }

        public double ISSlopeOfBaseline { get; init; }

        public string ISProcessingAlgorithm { get; init; }

        public double ISPeakAsymmetry { get; init; }

        public bool UseRecord { get; init; }

        public bool RecordModified { get; init; }

        public double CalculatedConcentration { get; init; }

        public double RelativeRetentionTime { get; init; }

        public double? Accuracy { get; init; }

        public double? ResponseFactor1 { get; init; }

        public double? ResponseFactor2 { get; init; }

        public double? ResponseFactor3 { get; init; }
    }
}
