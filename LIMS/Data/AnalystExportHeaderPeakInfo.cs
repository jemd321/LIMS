namespace LIMS.Data
{
    public readonly record struct AnalystExportHeaderPeakInfo
    {
        public string PeakName { get; init; }
        public bool IsInternalStandard { get; init; }

        public string InternalStandard { get; init; }
        public TransitionMRM TransitionMRM { get; init; }
    }
}
