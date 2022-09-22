using System;
using System.Collections.ObjectModel;

namespace LIMS.ViewModel
{
    /// <summary>
    /// ViewModel to represent standard and QC data as part of the regression.
    /// </summary>
    public interface IRegressionDataViewModel
    {
        /// <summary>
        /// Handles events that signal that the regression should be run again, and the child viewModels refreshed.
        /// </summary>
        public event EventHandler RegressionUpdated;

        /// <summary>
        /// Gets an Observable Collection of the standards exposed as viewModels.
        /// </summary>
        public ObservableCollection<RegressionDataItemViewModel> Standards { get; }

        /// <summary>
        /// Gets an Observable Collection of the QCs exposed as viewModels.
        /// </summary>
        public ObservableCollection<RegressionDataItemViewModel> QualityControls { get; }
    }
}
