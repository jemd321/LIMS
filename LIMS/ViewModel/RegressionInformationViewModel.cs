using System;
using System.Collections.Generic;
using LIMS.CustomEvent;
using LIMS.Enums;

namespace LIMS.ViewModel
{
    /// <summary>
    /// ViewModel to handle regression information.
    /// </summary>
    public class RegressionInformationViewModel : ViewModelBase, IRegressionInformationViewModel
    {
        private RegressionType _selectedRegressionType;
        private WeightingFactor _selectedWeightingFactor;
        private string _aTerm;
        private string _bTerm;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegressionInformationViewModel"/> class.
        /// </summary>
        /// <param name="regressionType">The type of the regression - eg. linear.</param>
        /// <param name="weightingFactor">The weighting factor applied to the regression - eg 1/x.</param>
        /// <param name="aTerm">The gradient of the regression equation.</param>
        /// <param name="bTerm">The y intercept of the regression.</param>
        public RegressionInformationViewModel(
            RegressionType regressionType,
            WeightingFactor weightingFactor,
            double aTerm,
            double bTerm)
        {
            _selectedRegressionType = regressionType;
            _selectedWeightingFactor = weightingFactor;
            ATerm = $"{aTerm:F3}";
            BTerm = $"{bTerm:F3}";
        }

        /// <summary>
        /// Event handler for the regression information changed event, which signals that the user has changed the regression type or weighting factor.
        /// </summary>
        public event EventHandler<RegressionInformationChangedEventArgs> RegressionInformationChanged;

        /// <summary>
        /// Gets a dictionary that displays the available regression types with a caption that can be displayed by a combobox.
        /// </summary>
        public Dictionary<string, RegressionType> RegressionTypesCaptioned { get; } = new()
        {
            // Add other regression types when implemented.
            { "Linear", RegressionType.Linear },
        };

        /// <summary>
        /// Gets a dictionary that displays the available weighting factors with a caption that can be displayed by a combobox.
        /// </summary>
        public Dictionary<string, WeightingFactor> WeightingFactorsCaptioned { get; } = new()
        {
            // Add other weighting factors when implemented.
            { "Unweighted", WeightingFactor.Unweighted },
            { "1/x^1/2", WeightingFactor.OneOverXHalf },
            { "1/x", WeightingFactor.OneOverX },
            { "1/x\u00B2", WeightingFactor.OneOverXSquared },
            { "1/y^1/2", WeightingFactor.OneOverYHalf },
            { "1/y", WeightingFactor.OneOverY },
            { "1/y\u00B2", WeightingFactor.OneOverYSquared },
        };

        /// <summary>
        /// Gets or sets the currently selected type of the regression.
        /// </summary>
        public RegressionType SelectedRegressionType
        {
            get => _selectedRegressionType;
            set
            {
                if (_selectedRegressionType != value)
                {
                    RaiseRegressionInformationChanged(
                        new RegressionInformationChangedEventArgs { RegressionType = value, WeightingFactor = SelectedWeightingFactor });
                }

                _selectedRegressionType = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the currently selected weighting factor applied to the regression.
        /// </summary>
        public WeightingFactor SelectedWeightingFactor
        {
            get => _selectedWeightingFactor;
            set
            {
                if (_selectedWeightingFactor != value)
                {
                    RaiseRegressionInformationChanged(
                        new RegressionInformationChangedEventArgs { RegressionType = SelectedRegressionType, WeightingFactor = value });
                }

                _selectedWeightingFactor = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets the equation of the regression based on the selected regression type.
        /// </summary>
        public string RegressionEquation => SelectedRegressionType switch
        {
            // Add equations for further regression types when implemented.
            RegressionType.Linear => "y = ax + b",
            _ => "Unknown",
        };

        /// <summary>
        /// Gets or sets the 'A' term of the regression equation.
        /// </summary>
        /// <remarks>In a linear regression the A term is the gradient.</remarks>
        public string ATerm
        {
            get => _aTerm;
            set
            {
                _aTerm = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the 'B' term of the regression equation.
        /// </summary>
        /// <remarks>In a linear regression the B term is the Y intercept.</remarks>
        public string BTerm
        {
            get => _bTerm;
            set
            {
                _bTerm = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Raises the regression information changed event, signalling that the user has selected a different regression type or weighing factor.
        /// </summary>
        /// <param name="e">The <see cref="RegressionInformationChangedEventArgs"/> object containing the regression type and weighting factor changes selected by the user.</param>
        protected virtual void RaiseRegressionInformationChanged(RegressionInformationChangedEventArgs e)
        {
            RegressionInformationChanged?.Invoke(this, e);
        }
    }
}
