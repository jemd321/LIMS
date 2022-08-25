using LIMS.Enums;
using LIMS.Model.RegressionModels;

namespace LIMS.Model
{
    public record AnalyticalRun
    {
        public AnalyticalRun(string analyticalRunID,
            string parentProjectID,
            RegressionData regressionData,
            RegressionType regressionType = RegressionType.Linear,
            WeightingFactor weightingFactor = WeightingFactor.OneOverXSquared)
        {
            AnalyticalRunID = analyticalRunID;
            ParentProjectID = parentProjectID;
            RegressionData = regressionData;
            RegressionType = regressionType;
            WeightingFactor = weightingFactor;
        }

        public string AnalyticalRunID { get; set; }
        public string ParentProjectID { get; }
        public RegressionData RegressionData { get; }
        public RegressionType RegressionType { get; }
        public WeightingFactor WeightingFactor { get; }

    }
}
