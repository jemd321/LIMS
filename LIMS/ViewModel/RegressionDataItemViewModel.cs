using LIMS.Enums;

namespace LIMS.ViewModel
{
    public class RegressionDataItemViewModel : ViewModelBase
    {
        // refactor to take from model rather than store in private backing field.
        private int _sampleNumber;
        private string _sampleName;
        private SampleType _sampleType;
        private double? _instrumentResponse;
        private double? _accuracy;
        private bool _isActive;
        private double? _calculatedConcentration;
        private double? _nominalConcentration;

        public int SampleNumber
        {
            get => _sampleNumber;
            set
            {
                _sampleNumber = value;
                RaisePropertyChanged();
            }
        }

        public string SampleName
        {
            get => _sampleName;
            set
            {
                _sampleName = value;
                RaisePropertyChanged();
            }
        }

        public SampleType SampleType
        {
            get => _sampleType;
            set
            {
                _sampleType = value;
                RaisePropertyChanged();
            }
        }

        public double? InstrumentResponse
        {
            get => _instrumentResponse;
            set
            {
                _instrumentResponse = value;
                RaisePropertyChanged();
            }
        }

        public double? CalculatedConcentration
        {
            get => _calculatedConcentration;
            set
            {
                _calculatedConcentration = value;
                RaisePropertyChanged();
            }
        }
        public double? NominalConcentration
        {
            get => _nominalConcentration;
            set
            {
                _nominalConcentration = value;
                RaisePropertyChanged();
            }
        }

        public double? Accuracy
        {
            get => _accuracy;
            set
            {
                _accuracy = value;
                RaisePropertyChanged();
            }
        }

        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                RaisePropertyChanged();
            }
        }





    }
}
