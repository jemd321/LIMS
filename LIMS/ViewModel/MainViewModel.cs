using LIMS.Command;
using LIMS.Data;
using LIMS.Dialog;
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
        private readonly IDialogService _dialogService;

        public MainViewModel(
            RegressionViewModel regressionViewModel,
            IFileDataService fileDataService, 
            IDialogService dialogService)
        {
            _fileDataService = fileDataService;
            _dialogService = dialogService;
            RegressionViewModel = regressionViewModel;

            CreateNewProjectCommand = new DelegateCommand(CreateNewProject);
            OpenAnalyticalRunCommand = new DelegateCommand(OpenAnalyticalRun);
            ImportAnalystFileCommand = new DelegateCommand(ImportAnalystFile);
            SaveAnalyticalRunCommand = new DelegateCommand(SaveAnalyticalRun);
        }

        public override void Load()
        {
            _fileDataService.SetupApplicationStorage();
            Projects = _fileDataService.LoadProjects();
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
            _dialogService.ShowActionDialog<ProjectCreationDialogViewModel>(result =>
            {
                // here we supply the function that will be executed when the closed event is fired, with the BOOL reslt of the dialog as a string?
                // perhaps we should just have Action<Bool>
                // we could create an accept event handler that exectues when the open but is clicked. THis assigns the VM string output property to a callback of Func
                // Dialog result not needed as changes are made directly on the filesystem by dialog
            });
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
    }
}
