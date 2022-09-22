using System;
using LIMS.CustomEvent;
using LIMS.Enums;

namespace LIMS.ViewModel
{
    /// <summary>
    /// ViewModel to expose individual data rows for samples, for display in a datagrid control.
    /// </summary>
    public class RegressionDataItemViewModel : ViewModelBase
    {
        private int _sampleNumber;
        private string _sampleName;
        private SampleType _sampleType;
        private string _instrumentResponse;
        private string _calculatedConcentration;
        private string _nominalConcentration;
        private string _accuracy;
        private bool _isActive;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegressionDataItemViewModel"/> class.
        /// </summary>
        /// <param name="sampleNumber">The sequence number of the sample - ie. the position in the order that the run was acquired.</param>
        /// <param name="sampleName">The name of the sample.</param>
        /// <param name="sampleType">The category of the sample.</param>
        /// <param name="instrumentResponse">The instrument response of the sample - ie. the peak area.</param>
        /// <param name="calculatedConcentration">The concentration calculated by the regression.</param>
        /// <param name="nominalConcentration">The known concentration that the sample was prepared at.</param>
        /// <param name="accuracy">The bias of the sample relative to the nominal concentration in percent.</param>
        /// <param name="isActive">A value indicating whether the sample should be included in the regression and/or statistical calculations.</param>
        public RegressionDataItemViewModel(
            int sampleNumber,
            string sampleName,
            SampleType sampleType,
            string instrumentResponse,
            string calculatedConcentration,
            string nominalConcentration,
            string accuracy,
            bool isActive)
        {
            SampleNumber = sampleNumber;
            SampleName = sampleName;
            SampleType = sampleType;
            InstrumentResponse = instrumentResponse;
            CalculatedConcentration = calculatedConcentration;
            NominalConcentration = nominalConcentration;
            Accuracy = accuracy;
            IsActive = isActive;
        }

        /// <summary>
        /// Event handler for an event signalling that the user has activated or deactivated a standard or QC.
        /// </summary>
        public event EventHandler<RegressionChangedEventArgs> RegressionDataChanged;

        /// <summary>
        /// Gets the sequence number of the sample - ie. the position in the order that the run was acquired.
        /// </summary>
        public int SampleNumber
        {
            get => _sampleNumber;
            private set
            {
                _sampleNumber = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets the name of the sample.
        /// </summary>
        public string SampleName
        {
            get => _sampleName;
            private set
            {
                _sampleName = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets the category of the sample.
        /// </summary>
        public SampleType SampleType
        {
            get => _sampleType;
            private set
            {
                _sampleType = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets the instrument response of the sample - ie. the peak area.
        /// </summary>
        public string InstrumentResponse
        {
            get => _instrumentResponse;
            private set
            {
                _instrumentResponse = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets the concentration calculated by the regression.
        /// </summary>
        public string CalculatedConcentration
        {
            get => _calculatedConcentration;
            private set
            {
                _calculatedConcentration = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets the known concentration that the sample was prepared at.
        /// </summary>
        public string NominalConcentration
        {
            get => _nominalConcentration;
            private set
            {
                _nominalConcentration = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets the bias of the sample relative to the nominal concentration in percent.
        /// </summary>
        public string Accuracy
        {
            get => _accuracy;
            private set
            {
                _accuracy = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the sample should be included in the regression and/or statistical calculations.
        /// </summary>
        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                RaisePropertyChanged();
                RaiseRegressionDataChanged(new RegressionChangedEventArgs { SampleNumber = SampleNumber, IsActive = IsActive });
            }
        }

        /// <summary>
        /// Raises an event signalling that the user has activated or deactivated a standard or QC.
        /// </summary>
        /// <param name="e">event args.</param>
        protected virtual void RaiseRegressionDataChanged(RegressionChangedEventArgs e)
        {
            e.SampleNumber = SampleNumber;
            e.IsActive = IsActive;
            RegressionDataChanged?.Invoke(this, e);
        }
    }
}
