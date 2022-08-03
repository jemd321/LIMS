namespace LIMS.Model
{
    public record AnalyticalRun
    {
        public AnalyticalRun(string analyticalRunID, string parentProjectID, RegressionData regressionData)
        {
            
            AnalyticalRunID = analyticalRunID;
            ParentProjectID = parentProjectID;
            RegressionData = regressionData;
        }

        public string AnalyticalRunID { get; }
        public string ParentProjectID { get; }
        public RegressionData RegressionData { get; }

    }
}
