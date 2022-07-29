using LIMS.Command;
using LIMS.Data;
using Microsoft.Win32;
using System.IO;

namespace LIMS.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase _selectedRegressionViewModel;
        private readonly DataImporter _dataImporter;

        public MainViewModel(RegressionViewModel regressionViewModel, DataImporter dataImporter)
        {
            _dataImporter = dataImporter;
            RegressionViewModel = regressionViewModel;

            ImportAnalystFileCommand = new DelegateCommand(ImportAnalystFile);
        }

        private async void ImportAnalystFile(object parameter)
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Text documents (.txt)|*.txt";

            bool? result = fileDialog.ShowDialog();
            string selectedFile = "";
            if (result == true)
            {
                selectedFile = fileDialog.FileName;
            }
            if (!string.IsNullOrEmpty(selectedFile))
            {
                FileInfo validFilePath = _dataImporter.ValidateFilePath(selectedFile);
                string rawData = await _dataImporter.GetRawData(validFilePath);
                
                SelectedViewModel = (ViewModelBase)RegressionViewModel;
                await SelectedViewModel.Load(rawData);
            }
        }

        public ViewModelBase SelectedViewModel
        {
            get => _selectedRegressionViewModel;
            set
            {
                _selectedRegressionViewModel = value;
                RaisePropertyChanged();
            }
        }

        public RegressionViewModel RegressionViewModel { get; }

        public DelegateCommand ImportAnalystFileCommand { get; }


    }
}
