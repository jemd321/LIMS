namespace LIMS.Model
{
    public record AnalyticalRun
    {
        public AnalyticalRun(string analyticalRunID)
        {
            RunID = analyticalRunID;
        }

        public string RunID { get; }
        public RegressionData RunData { get; set; }
    }
}
