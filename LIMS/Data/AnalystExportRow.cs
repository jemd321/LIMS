﻿namespace LIMS.Data
{
    public readonly record struct AnalystExportRow
    {
        public readonly string SampleName { get; init; }
        public readonly int SampleID { get; init; }
        public readonly string FileType { get; init; }
        public readonly string SampleDescription { get; init; }
        public readonly string SetNumber { get; init; }
        public readonly string AcquisitonMethod { get; init; }
        public readonly string AcquisitionDate { get; init; }
        public readonly string RackType { get; init; }
        public readonly string Vial { get; init; }
        public readonly string Position { get; init; }
        public readonly string PlateType { get; init; }
        public readonly string PlateNumber { get; init; }
        public readonly string FileName { get; init; }
        public readonly string DilutionFactor { get; init; }
        public readonly string WeightToVolumeRatio { get; init; }
        public readonly string SampleAnnotation { get; init; }
        public readonly string PeakName { get; init; }
        public readonly string Units { get; init; }
        public readonly string Area { get; init; }
        public readonly string AnalytePeakHeightForDAD { get; init; }
        public readonly string Height { get; init; }
        public readonly string AnalyteAnnotation { get; init; }
        public readonly string Concentration { get; init; }
        public readonly string RetentionTime { get; init; }
        public readonly string ExpectedRetentionTime { get; init; }
        public readonly string RetentionTimeWindow { get; init; }
        public readonly string CentroidLocation { get; init; }
        public readonly string StartScan { get; init; }
        public readonly string StartTime { get; init; }
        public readonly string EndScan { get; init; }
        public readonly string EndTime { get; init; }
        public readonly string IntegrationType { get; init; }
        public readonly string SignalToNoiseRatio { get; init; }
        public readonly string PeakWidth { get; init; }
        public readonly string StandardQueryStatus { get; init; }
        public readonly string AnalyteMassRange { get; init; }
        public readonly string AnalytePeakAreaForDAD { get; init; }
        public readonly string AnalyteToISAreaRatio { get; init; }
        public readonly string AnalyteToISHeightRatio { get; init; }
        public readonly string AnalyteWavelengthRanges { get; init; }
        public readonly string AnalyteUVRange { get; init; }
        public readonly string AnalytePeakWidthAtHalfHeight { get; init; }
        public readonly string AnalyteSlopeOfBaseline { get; init; }
        public readonly string AnalyteProcessingAlgorithm { get; init; }
        public readonly string AnalytePeakAsymmetry { get; init; }
        public readonly string ISPeakName { get; init; }
        public readonly string ISUnits { get; init; }
        public readonly string ISArea { get; init; }
        public readonly string ISPeakAreaForDAD { get; init; }
        public readonly string ISHeight { get; init; }
        public readonly string ISConcentration { get; init; }
        public readonly string ISRetentionTime { get; init; }
        public readonly string ISExpectedRetentionTime { get; init; }
        public readonly string ISRetentionTimeWindow { get; init; }
        public readonly string ISCentroidLocation { get; init; }
        public readonly string ISStartScan { get; init; }
        public readonly string ISStartTime { get; init; }
        public readonly string ISStopScan { get; init; }
        public readonly string ISEndTime { get; init; }
        public readonly string ISIntegrationType { get; init; }
        public readonly string ISSignalToNoiseRatio { get; init; }
        public readonly string ISPeakWidth { get; init; }
        public readonly string ISMassRange { get; init; }
        public readonly string ISWaveLengthRanges { get; init; }
        public readonly string ISUVRange { get; init; }
        public readonly string ISPeakWidthAtHalfHeight { get; init; }
        public readonly string ISSlopeOfBaseline { get; init; }
        public readonly string ISProcessingAlgorithm { get; init; }
        public readonly string ISPeakAsymmetry { get; init; }
        public readonly string UseRecord { get; init; }
        public readonly string RecordModified { get; init; }
        public readonly string CalculatedConcentrationForDAD { get; init; }
        public readonly string RelativeRetentionTime { get; init; }
        public readonly string Accuracy { get; init; }
        public readonly string ResponseFactor1 { get; init; }
        public readonly string ResponseFactor2 { get; init; }
        public readonly string ResponseFactor3 { get; init; }
    }
}
