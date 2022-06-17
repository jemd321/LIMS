using LIMS.Enums;

namespace LIMS.ViewModel
{
    public class RegressionDataItemViewModel : ViewModelBase
    {
        private int _sampleNumber;
        private string _sampleName;
        private SampleType _sampleType;
        private double? _instrumentResponse;
        private double? _bias;
        private bool _includeInRegression;

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

        public double? Bias
        {
            get => _bias;
            set
            {
                _bias = value;
                RaisePropertyChanged();
            }
        }

        public bool IncludeInRegression
        {
            get => _includeInRegression;
            set
            {
                _includeInRegression = value;
                RaisePropertyChanged();
            }
        }





    }
}
