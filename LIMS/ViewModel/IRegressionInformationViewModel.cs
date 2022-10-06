using System;
using LIMS.CustomEvent;
using LIMS.Enums;

namespace LIMS.ViewModel
{
    /// <summary>
    /// Interface for RegressionInformationViewModel.
    /// </summary>
    public interface IRegressionInformationViewModel
    {
        /// <summary>
        /// Event handler for the regression information changed event, which signals that the user has changed the regression type or weighting factor.
        /// </summary>
        public event EventHandler<RegressionInformationChangedEventArgs> RegressionInformationChanged;

        /// <summary>
        /// Gets or sets the currently selected type of the regression.
        /// </summary>
        RegressionType SelectedRegressionType { get; set; }

        /// <summary>
        /// Gets or sets the currently selected weighting factor applied to the regression.
        /// </summary>
        WeightingFactor SelectedWeightingFactor { get; set; }

        /// <summary>
        /// Gets the equation of the regression based on the selected regression type.
        /// </summary>
        public string RegressionEquation { get; }

        /// <summary>
        /// Gets or sets the 'A' term of the regression equation.
        /// </summary>
        /// <remarks>In a linear regression the A term is the gradient.</remarks>
        string ATerm { get; set; }

        /// <summary>
        /// Gets or sets the 'B' term of the regression equation.
        /// </summary>
        /// <remarks>In a linear regression the B term is the Y intercept.</remarks>
        string BTerm { get; set; }
    }
}