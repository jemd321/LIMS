using LIMS.Enums;
using System.Collections.Generic;

namespace LIMS.ViewModel
{
    /// <summary>
    /// ViewModel to handle regression information.
    /// </summary>
    public class RegressionInformationViewModel : ViewModelBase, IRegressionInformationViewModel
    {
        private RegressionType _selectedRegressionType;
        private WeightingFactor _selectedWeightingFactor;
        private double _aTerm;
        private double _bTerm;

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
            _aTerm = aTerm;
            _bTerm = bTerm;
        }

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
            { "1/x\u00B2", WeightingFactor.OneOverXSquared },
        };

        /// <summary>
        /// Gets or sets the currently selected type of the regression.
        /// </summary>
        public RegressionType SelectedRegressionType
        {
            get => _selectedRegressionType;
            set
            {
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
                _selectedWeightingFactor = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the 'A' term of the regression equation.
        /// </summary>
        /// <remarks>In a linear regression the A term is the gradient.</remarks>
        public double ATerm
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
        public double BTerm
        {
            get => _bTerm;
            set
            {
                _bTerm = value;
                RaisePropertyChanged();
            }
        }
    }
}
