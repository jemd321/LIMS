using System;
using System.Collections.ObjectModel;
using System.IO;
using LIMS.Command;
using LIMS.Data;
using LIMS.Dialog;
using LIMS.Model;
using LIMS.ViewModel.DialogViewModel;

namespace LIMS.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataProvider _fileDataService;
        private readonly IDataImporter _dataImporter;
        private readonly IDialogService _dialogService;
        private ViewModelBase _selectedRegressionViewModel;
        private Project _selectedProject;
        private ObservableCollection<Project> _projects;

        public MainViewModel(
            IRegressionViewModel regressionViewModel,
            IDataProvider fileDataService,
            IDataImporter dataImporter,
            IDialogService dialogService)
        {
            _fileDataService = fileDataService;
            _dataImporter = dataImporter;
            _dialogService = dialogService;
            RegressionViewModel = regressionViewModel;

            CreateNewProjectCommand = new DelegateCommand(CreateNewProject);
            OpenAnalyticalRunCommand = new DelegateCommand(OpenAnalyticalRun, CanOpenAnalyticalRunExecute);
            ImportAnalystFileCommand = new DelegateCommand(ImportAnalystFile);
            SaveAnalyticalRunCommand = new DelegateCommand(SaveAnalyticalRun);
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

        public Project SelectedProject
        {
            get => _selectedProject;
            set
            {
                _selectedProject = value;
                RaisePropertyChanged();
                OpenAnalyticalRunCommand.RaiseCanExecuteChanged();
            }
        }

        public IRegressionViewModel RegressionViewModel { get; }

        public ObservableCollection<Project> Projects
        {
            get => _projects;
            private set
            {
                _projects = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand CreateNewProjectCommand { get; }

        public DelegateCommand OpenAnalyticalRunCommand { get; }

        public DelegateCommand ImportAnalystFileCommand { get; }

        public DelegateCommand SaveAnalyticalRunCommand { get; }

        public override void Load()
        {
            _fileDataService.SetupApplicationStorage();
            Projects = _fileDataService.LoadProjects();
        }

        private bool CanOpenAnalyticalRunExecute(object parameter)
        {
            return SelectedProject is not null;
        }

        private void CreateNewProject(object parameter)
        {
            _dialogService.ShowActionDialog<ProjectCreationDialogViewModel>(result => { });
            Projects = _fileDataService.LoadProjects();
        }

        private void OpenAnalyticalRun(object parameter)
        {
            string selectedAnalyticalRunID = string.Empty;
            _dialogService.ShowStringIODialog<OpenAnalyticalRunDialogViewModel>(result => { }, dialogInput: SelectedProject.ProjectID, output =>
             {
                 selectedAnalyticalRunID = output;
             });
            var openedAnalyticalRun = _fileDataService.LoadAnalyticalRun(SelectedProject, selectedAnalyticalRunID);
            SelectedViewModel = (ViewModelBase)RegressionViewModel;
            SelectedViewModel.Load(openedAnalyticalRun);
        }

        private void SaveAnalyticalRun(object parameter)
        {
            AnalyticalRun currentlyOpenRun = RegressionViewModel.OpenAnalyticalRun;
            if (currentlyOpenRun.AnalyticalRunID == string.Empty)
            {
                string userSelectedAnalyticalRunID = string.Empty;
                _dialogService.ShowStringIODialog<SaveAnalyticalRunDialogViewModel>(result => { }, dialogInput: SelectedProject.ProjectID, output =>
                 {
                     userSelectedAnalyticalRunID = output;
                     currentlyOpenRun.AnalyticalRunID = userSelectedAnalyticalRunID;
                 });
                _fileDataService.SaveAnalyticalRun(currentlyOpenRun);
            }
            else
            {
                _fileDataService.SaveAnalyticalRun(currentlyOpenRun);
            }
        }

        private void ImportAnalystFile(object parameter)
        {
            string selectedFile = _dialogService.ShowOpenFileDialog();
            if (string.IsNullOrEmpty(selectedFile))
            {
                throw new NotImplementedException();

                // Handle gracefully with message box
            }

            FileInfo validFilePath = _fileDataService.ValidateFilePath(selectedFile);
            string rawData = _fileDataService.GetRawData(validFilePath);
            var regressionData = _dataImporter.ParseImportedRawData(rawData);
            var analyticalRun = new AnalyticalRun(analyticalRunID: string.Empty, SelectedProject.ProjectID, regressionData);
            SelectedViewModel = (ViewModelBase)RegressionViewModel;
            SelectedViewModel.Load(analyticalRun);
        }
    }
}
