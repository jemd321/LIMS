namespace LIMS.Model
{
    public record AnalyticalRun
    {
        public AnalyticalRun(string analyticalRunID, string parentProjectID)
        {
            
            AnalyticalRunID = analyticalRunID;
            ParentProjectID = parentProjectID;
        }

        public string AnalyticalRunID { get; }
        public string ParentProjectID { get; }

    }
}
