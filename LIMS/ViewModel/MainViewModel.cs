using LIMS.Command;
using LIMS.Data;
using LIMS.Model;
using Microsoft.Win32;
using System;
using System.IO;

namespace LIMS.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase _selectedRegressionViewModel;

        public MainViewModel()
        {
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
                var regressionDataProvider = new AnalystFileImporter(selectedFile);
                SelectedRegressionViewModel = new RegressionViewModel(regressionDataProvider);
                await SelectedRegressionViewModel.Load();
            }

        }
        public ViewModelBase SelectedRegressionViewModel
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
