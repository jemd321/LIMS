using System.Collections.ObjectModel;
using LIMS.Command;
using LIMS.Data;
using LIMS.Dialog;
using LIMS.Enums;
using LIMS.Model;
using LIMS.ViewModel.DialogViewModel;

namespace LIMS.ViewModel
{
    /// <summary>
    /// ViewModel for the <see cref="MainWindow.xaml"/>.
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;
        private readonly IDataImporter _dataImporter;
        private readonly IDialogService _dialogService;
        private ViewModelBase _selectedRegressionViewModel;
        private Project _selectedProject;
        private ObservableCollection<Project> _projects;
        private string _openAnalyticalRunName;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        /// <param name="regressionViewModel">The <see cref="RegressionViewModel"/> that will be filled with a regression to be displayed.</param>
        /// <param name="dataService">The <see cref="IDataService"/> service used save and load data to storage.</param>
        /// <param name="dataImporter">The <see cref="IDataImporter"/> service used to import data from external programs.</param>
        /// <param name="dialogService">The <see cref="IDialogService"/> used to create and show any dialogs in an MVVM fashion.</param>
        public MainViewModel(
            IRegressionViewModel regressionViewModel,
            IDataService dataService,
            IDataImporter dataImporter,
            IDialogService dialogService)
        {
            _dataService = dataService;
            _dataImporter = dataImporter;
            _dialogService = dialogService;
            RegressionViewModel = regressionViewModel;

            CreateNewProjectCommand = new DelegateCommand(CreateNewProject);
            OpenAnalyticalRunCommand = new DelegateCommand(OpenAnalyticalRun, CanOpenAnalyticalRunExecute);
            ImportAnalystFileCommand = new DelegateCommand(ImportAnalystFile, CanImportAnalystFileExecute);
            SaveAnalyticalRunCommand = new DelegateCommand(SaveAnalyticalRun, CanSaveAnalyticalRunExecute);
        }

        /// <summary>
        /// Gets or sets current VM and corresponding view to be displayed in the mainWindow.
        /// </summary>
        public ViewModelBase SelectedViewModel
        {
            get => _selectedRegressionViewModel;
            set
            {
                _selectedRegressionViewModel = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the currently selected project, raising canExecuteChange on other project bar buttons.
        /// </summary>
        public Project SelectedProject
        {
            get => _selectedProject;
            set
            {
                _selectedProject = value;
                RaisePropertyChanged();

                // Update the execution status of the other buttons - we don't want to be able to do anything without a project open.
                OpenAnalyticalRunCommand.RaiseCanExecuteChanged();
                ImportAnalystFileCommand.RaiseCanExecuteChanged();
                SaveAnalyticalRunCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Gets the currently loaded <see cref="RegressionViewModel"/>.
        /// </summary>
        public IRegressionViewModel RegressionViewModel { get; }

        /// <summary>
        /// Gets a list of the projects that have been loaded from storage on startup.
        /// </summary>
        public ObservableCollection<Project> Projects
        {
            get => _projects;
            private set
            {
                _projects = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the currently open Analytical Run Name.
        /// </summary>
        public string OpenAnalyticalRunName
        {
            get => _openAnalyticalRunName;
            set
            {
                _openAnalyticalRunName = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets the command to open the project creation and deletion dialog.
        /// </summary>
        public DelegateCommand CreateNewProjectCommand { get; }

        /// <summary>
        /// Gets the command to open the openAnalyticalRun Dialog, allowing user to open a run from a project.
        /// </summary>
        public DelegateCommand OpenAnalyticalRunCommand { get; }

        /// <summary>
        /// Gets the command to open the importAnalystFile dialog, allowing user to import external data.
        /// </summary>
        public DelegateCommand ImportAnalystFileCommand { get; }

        /// <summary>
        /// Gets the command to open the saveAnalyticalRun dialog, allowing user to save a run.
        /// </summary>
        public DelegateCommand SaveAnalyticalRunCommand { get; }

        /// <inheritdoc/>
        public override void Load()
        {
            // Configure the application storage and load all projects.
            _dataService.SetupApplicationStorage();
            Projects = _dataService.LoadProjects();
        }

        private bool CanOpenAnalyticalRunExecute(object parameter)
        {
            return SelectedProject is not null;
        }

        private bool CanImportAnalystFileExecute(object arg)
        {
            return SelectedProject is not null;
        }

        private bool CanSaveAnalyticalRunExecute(object arg)
        {
            return SelectedProject is not null && RegressionViewModel.OpenAnalyticalRun is not null;
        }

        private void CreateNewProject(object parameter)
        {
            _dialogService.ShowActionDialog<ProjectCreationDialogViewModel>(result => { });
            Projects = _dataService.LoadProjects();
        }

        private void OpenAnalyticalRun(object parameter)
        {
            string selectedAnalyticalRunID = string.Empty;
            _dialogService.ShowStringIODialog<OpenAnalyticalRunDialogViewModel>(result => { }, dialogInput: SelectedProject.ProjectID, output =>
             {
                 selectedAnalyticalRunID = output;
             });

            if (string.IsNullOrEmpty(selectedAnalyticalRunID))
            {
                // No choice made by user - do nothing.
                return;
            }

            OpenAnalyticalRunName = selectedAnalyticalRunID;
            var openedAnalyticalRun = _dataService.LoadAnalyticalRun(SelectedProject, selectedAnalyticalRunID);
            SelectedViewModel = (ViewModelBase)RegressionViewModel;
            SelectedViewModel.Load(openedAnalyticalRun);

            // Update availability of save button
            SaveAnalyticalRunCommand.RaiseCanExecuteChanged();
        }

        private void SaveAnalyticalRun(object parameter)
        {
            AnalyticalRun currentlyOpenRun = RegressionViewModel.OpenAnalyticalRun;
            if (currentlyOpenRun.AnalyticalRunID == string.Empty)
            {
                // If there is no run name the user is prompted to choose a name via dialog.
                string userSelectedAnalyticalRunID = string.Empty;
                _dialogService.ShowStringIODialog<SaveAnalyticalRunDialogViewModel>(result => { }, dialogInput: SelectedProject.ProjectID, output =>
                 {
                     userSelectedAnalyticalRunID = output;
                     currentlyOpenRun.AnalyticalRunID = userSelectedAnalyticalRunID;
                 });
                _dataService.SaveAnalyticalRun(currentlyOpenRun);
                OpenAnalyticalRunName = currentlyOpenRun.AnalyticalRunID;
            }
            else
            {
                // The run already exists and can be saved over freely.
                _dataService.SaveAnalyticalRun(currentlyOpenRun);
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

            var dataReadResult = _dataService.ReadDataFromTextFile(selectedFilePath);
            if (dataReadResult.IsSuccess)
            {
                // File read successfully, continue and parse data.
                var parsedData = _dataImporter.ParseImportedRawData(dataReadResult.Data);
                if (parsedData.IsSuccess)
                {
                    var analyticalRun = new AnalyticalRun(analyticalRunID: string.Empty, SelectedProject.ProjectID, parsedData.Data);
                    SelectedViewModel = (ViewModelBase)RegressionViewModel;
                    SelectedViewModel.Load(analyticalRun);
                }
                else
                {
                    // Inform user that parsing failed with error message.
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
                // File read error, inform user of error.
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

            // Update execution status of save command so that successfully imported data can be saved.
            SaveAnalyticalRunCommand.RaiseCanExecuteChanged();
        }
    }
}
