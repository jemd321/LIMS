namespace LIMS.Model
{
    public record AnalyticalRun
    {
        public AnalyticalRun(string runID, RegressionData runData)
        {
            RunID = runID;
            RunData = runData;
        }

        public string RunID { get; }
        public RegressionData RunData { get; }
    }
}
