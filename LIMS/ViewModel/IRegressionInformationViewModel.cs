using LIMS.Enums;

namespace LIMS.ViewModel
{
    public interface IRegressionInformationViewModel
    {
        double ATerm { get; set; }
        RegressionType SelectedRegressionType { get; set; }
        WeightingFactor SelectedWeightingFactor { get; set; }
        double BTerm { get; set; }
    }
}