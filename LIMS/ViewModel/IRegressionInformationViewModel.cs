using LIMS.Enums;

namespace LIMS.ViewModel
{
    public interface IRegressionInformationViewModel
    {
        double Gradient { get; set; }
        RegressionType RegressionType { get; set; }
        WeightingFactor WeightingFactor { get; set; }
        double YIntercept { get; set; }
    }
}