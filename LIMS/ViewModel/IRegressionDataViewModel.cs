using System.Collections.ObjectModel;

namespace LIMS.ViewModel
{
    /// <summary>
    /// ViewModel to represent standard and QC data as part of the regression.
    /// </summary>
    public interface IRegressionDataViewModel
    {
        public ObservableCollection<RegressionDataItemViewModel> Standards { get; }

        public ObservableCollection<RegressionDataItemViewModel> QualityControls { get; }
    }
}
