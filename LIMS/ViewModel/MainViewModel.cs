using System;
using System.Collections.ObjectModel;
using System.IO;
using LIMS.Command;
using LIMS.Data;
using LIMS.Dialog;
using LIMS.Enums;
using LIMS.Model;
using LIMS.ViewModel.DialogViewModel;

namespace LIMS.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataService _fileDataService;
        private readonly IDataImporter _dataImporter;
        private readonly IDialogService _dialogService;
        private ViewModelBase _selectedRegressionViewModel;
        private Project _selectedProject;
        private ObservableCollection<Project> _projects;

        public MainViewModel(
            IRegressionViewModel regressionViewModel,
            IDataService fileDataService,
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
            string selectedFilePath = _dialogService.ShowOpenFileDialog();
            if (string.IsNullOrEmpty(selectedFilePath))
            {
                // No choice made by the user.
                return;
            }

            var dataReadResult = _fileDataService.ReadDataFromTextFile(selectedFilePath);
            if (dataReadResult.IsSuccess)
            {
                // TODO wrap this parse in try and handle errors gracefully with dialog.
                var parsedData = _dataImporter.ParseImportedRawData(dataReadResult.Data);
                if (parsedData.IsSuccess)
                {
                    var analyticalRun = new AnalyticalRun(analyticalRunID: string.Empty, SelectedProject.ProjectID, parsedData.Data);
                    SelectedViewModel = (ViewModelBase)RegressionViewModel;
                    SelectedViewModel.Load(analyticalRun);
                }
                else
                {
                    string errorMessage = parsedData.ParseFailureReason switch
                    {
                        ParseFailureReason.InvalidFileFormat => "The file you selected was not in a format that was recognised.",
                        ParseFailureReason.InvalidCast => "The application encountered an error in converting the data into a useable format.",
                        ParseFailureReason.OtherSystemException => "An error occured during the import of the data.",
                        _ => "An error occured during the import of the data.",
                    };
                    _dialogService.ShowStringIODialog<ErrorMessageDialogViewModel>(accepted => { }, dialogInput: errorMessage, dialogOutput => { });
                }
            }
            else
            {
                string errorMessage = dataReadResult.DataReadFailureReason switch
                {
                    DataReadFailureReason.PathTooLong => "The file you selected has a path that is too long. Rename it and try again.",
                    DataReadFailureReason.InvalidDirectory => "A directory was selected instead of a text file.",
                    DataReadFailureReason.UnauthorizedAccess => "You may not have the permissions required to access this file.",
                    DataReadFailureReason.FileNotFound => "The file you selected could not be found.",
                    DataReadFailureReason.NotSupported => "The file you selected has an invalid file path format.",
                    DataReadFailureReason.GenericArgumentProblem => "The file you selected contains only white space, or contains one or more invalid characters.",
                    DataReadFailureReason.GenericIOProblem => "The application could not open the file you selected",
                    _ => "The application could not open the file you selected",
                };
                _dialogService.ShowStringIODialog<ErrorMessageDialogViewModel>(accepted => { }, dialogInput: errorMessage, dialogOutput => { });
            }
        }
    }
}
