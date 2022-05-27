using LIMS.Enums;
using System;

namespace LIMS.Data
{
    public readonly record struct AnalystExportRow
    {
        public readonly string SampleName { get; init; }
        public readonly int SampleID { get; init; }
        public readonly SampleType SampleType { get; init; }
        public readonly string SampleDescription { get; init; }
        public readonly int SetNumber { get; init; }
        public readonly string AcquisitonMethod { get; init; }
        public readonly DateTime AcquisitionDate { get; init; }
        public readonly string RackType { get; init; }
        public readonly int RackNumber { get; init; }
        public readonly int VialPosition { get; init; }
        public readonly string PlateType { get; init; }
        public readonly int PlateNumber { get; init; }
        public readonly string FileName { get; init; }
        public readonly double DilutionFactor { get; init; }
        public readonly double WeightToVolumeRatio { get; init; }
        public readonly string SampleAnnotation1 { get; init; }
        public readonly string SampleAnnotation2 { get; init; }
        public readonly string PeakName { get; init; }
        public readonly Units Units { get; init; }
        public readonly double Area { get; init; }
        public readonly double Height { get; init; }
        public readonly string AnalyteAnnotation { get; init; }
        public readonly double NominalConcentration { get; init; }
        public readonly double RetentionTime { get; init; }
        public readonly double ExpectedRetentionTime { get; init; }
        public readonly double RetentionTimeWindow { get; init; }
        public readonly double CentroidLocation { get; init; }
        public readonly int StartScan { get; init; }
        public readonly double StartTime { get; init; }
        public readonly int EndScan { get; init; }
        public readonly double EndTime { get; init; }
        public readonly string IntegrationType { get; init; }
        public readonly double? SignalToNoiseRatio { get; init; }
        public readonly double PeakWidth { get; init; }
        public readonly string StandardQueryStatus { get; init; }
        public readonly TransitionMRM AnalyteTransitionMRM { get; init; }
        public readonly double? AnalyteToISAreaRatio { get; init; }
        public readonly double? AnalyteToISHeightRatio { get; init; }
        public readonly double AnalytePeakWidthAtHalfHeight { get; init; }
        public readonly double AnalyteSlopeOfBaseline { get; init; }
        public readonly string AnalyteProcessingAlgorithm { get; init; }
        public readonly double AnalytePeakAsymmetry { get; init; }
        public readonly string ISPeakName { get; init; }
        public readonly Units ISUnits { get; init; }
        public readonly double ISArea { get; init; }
        public readonly double ISHeight { get; init; }
        public readonly double ISConcentration { get; init; }
        public readonly double ISRetentionTime { get; init; }
        public readonly double ISExpectedRetentionTime { get; init; }
        public readonly double ISRetentionTimeWindow { get; init; }
        public readonly double ISCentroidLocation { get; init; }
        public readonly int ISStartScan { get; init; }
        public readonly double ISStartTime { get; init; }
        public readonly int ISStopScan { get; init; }
        public readonly double ISEndTime { get; init; }
        public readonly string ISIntegrationType { get; init; }
        public readonly double ISSignalToNoiseRatio { get; init; }
        public readonly double ISPeakWidth { get; init; }
        public readonly TransitionMRM ISTransitionMRM { get; init; }
        public readonly double ISPeakWidthAtHalfHeight { get; init; }
        public readonly double ISSlopeOfBaseline { get; init; }
        public readonly string ISProcessingAlgorithm { get; init; }
        public readonly double ISPeakAsymmetry { get; init; }
        public readonly bool UseRecord { get; init; }
        public readonly bool RecordModified { get; init; }
        public readonly double CalculatedConcentration { get; init; }
        public readonly double RelativeRetentionTime { get; init; }
        public readonly double? Accuracy { get; init; }
        public readonly double? ResponseFactor1 { get; init; }
        public readonly double? ResponseFactor2 { get; init; }
        public readonly double? ResponseFactor3 { get; init; }
    }
}
