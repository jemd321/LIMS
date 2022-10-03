using LIMS.Enums;

namespace LIMS.ViewModel
{
    /// <summary>
    /// ViewModel to handle regression information.
    /// </summary>
    public class RegressionInformationViewModel : ViewModelBase, IRegressionInformationViewModel
    {
        private RegressionType _regressionType;
        private WeightingFactor _weightingFactor;
        private double _gradient;
        private double _yIntercept;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegressionInformationViewModel"/> class.
        /// </summary>
        /// <param name="regressionType">The type of the regression - eg. linear.</param>
        /// <param name="weightingFactor">The weighting factor applied to the regression - eg 1/x.</param>
        /// <param name="gradient">The gradient of the regression equation.</param>
        /// <param name="yIntercept">The y intercept of the regression.</param>
        public RegressionInformationViewModel(
            RegressionType regressionType,
            WeightingFactor weightingFactor,
            double gradient,
            double yIntercept)
        {
            _regressionType = regressionType;
            _weightingFactor = weightingFactor;
            _gradient = gradient;
            _yIntercept = yIntercept;
        }

        /// <summary>
        /// Gets or sets the type of the regression.
        /// </summary>
        public RegressionType RegressionType
        {
            get => _regressionType;
            set
            {
                _regressionType = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the weighting factor applied to the regression.
        /// </summary>
        public WeightingFactor WeightingFactor
        {
            get => _weightingFactor;
            set
            {
                _weightingFactor = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the gradient of the regression equation.
        /// </summary>
        public double Gradient
        {
            get => _gradient;
            set
            {
                _gradient = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the y intercept of the regression equation.
        /// </summary>
        public double YIntercept
        {
            get => _yIntercept;
            set
            {
                _yIntercept = value;
                RaisePropertyChanged();
            }
        }
    }
}
