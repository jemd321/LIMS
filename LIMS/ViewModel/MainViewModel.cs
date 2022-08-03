using LIMS.Command;
using LIMS.Data;
using LIMS.Model;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LIMS.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase _selectedRegressionViewModel;
        private readonly IFileDataService _fileDataService;

        public MainViewModel(RegressionViewModel regressionViewModel, IFileDataService fileDataService)
        {
            _fileDataService = fileDataService;
            RegressionViewModel = regressionViewModel;

            CreateNewProjectCommand = new DelegateCommand(CreateNewProject);
            OpenAnalyticalRunCommand = new DelegateCommand(OpenAnalyticalRun);
            ImportAnalystFileCommand = new DelegateCommand(ImportAnalystFile);
            SaveAnalyticalRunCommand = new DelegateCommand(SaveAnalyticalRun);
        }

        public new void Load()
        {
            _fileDataService.SetupApplicationStorage();
            Projects = LoadProjectsList();
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
        public ObservableCollection<Project> Projects { get; private set; }
        public DelegateCommand CreateNewProjectCommand { get; }
        public DelegateCommand OpenAnalyticalRunCommand { get; }
        public DelegateCommand ImportAnalystFileCommand { get; }
        public DelegateCommand SaveAnalyticalRunCommand { get; }

        private void CreateNewProject(object parameter)
        {
            throw new NotImplementedException();
        }

        private void OpenAnalyticalRun(object parameter)
        {
            throw new NotImplementedException();
        }

        private void SaveAnalyticalRun(object parameter)
        {
            throw new NotImplementedException();
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
                FileInfo validFilePath = _fileDataService.ValidateFilePath(selectedFile);
                string rawData = await _fileDataService.GetRawData(validFilePath);

                SelectedViewModel = (ViewModelBase)RegressionViewModel;
                await SelectedViewModel.Load(rawData);
            }
        }

        private ObservableCollection<Project> LoadProjectsList()
        {
            var observableProjects = new ObservableCollection<Project>();
            _fileDataService.LoadProjects().ForEach(observableProjects.Add);
            return observableProjects;
        }
    }
}
