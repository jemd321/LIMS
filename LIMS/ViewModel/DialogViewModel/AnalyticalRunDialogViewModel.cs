using LIMS.Command;
using LIMS.Data;
using LIMS.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace LIMS.ViewModel.DialogViewModel
{
    public class AnalyticalRunDialogViewModel : ValidationViewModelBase, IDialogViewModel
    {
        private const int MAXPROJECTNAMELENGTH = 36;

        private readonly IFileDataService _fileDataService;
        private ObservableCollection<Project> _loadedAnalyticalRuns;
        private string _newProjectName;
        private Project _selectedAnalyticalRun;

        public AnalyticalRunDialogViewModel(IFileDataService fileDataService)
        {
            _fileDataService = fileDataService;

            OpenAnalyticalRunCommand = new DelegateCommand(OpenAnalyticalRun, CanOpenAnalyticalRun);
            DeleteAnalyticalRunCommand = new DelegateCommand(DeleteAnalyticalRun, CanDeleteAnalyticalRun);
        }

        public DelegateCommand OpenAnalyticalRunCommand { get; }
        public DelegateCommand DeleteAnalyticalRunCommand { get; }

        public ObservableCollection<Project> LoadedAnalyticalRuns
        {
            get { return _loadedAnalyticalRuns; }
            set { _loadedAnalyticalRuns = value; RaisePropertyChanged(); }
        }

        // In this dialog, the calling viewModel should supply the currently open ProjectID
        public string OptionalMessage { get; set; }
        public string OpenProjectID { get; private set; }

        public Project OpenProject { get; private set; }

        public Project SelectedAnalyticalRun
        {
            get { return _selectedAnalyticalRun; }
            set
            {
                _selectedAnalyticalRun = value;
                RaisePropertyChanged();
                DeleteAnalyticalRunCommand.RaiseCanExecuteChanged();
            }
        }

        public override void Load()
        {
            LoadedAnalyticalRuns = _fileDataService.LoadProjects();
            OpenProjectID = OptionalMessage;
            var openProject = _fileDataService
                .LoadProjects()
                .Select(p => p.ProjectID)
                .Where(pID => pID == OpenProjectID);
            OpenProject = OpenProject;
        }

        private void OpenAnalyticalRun(object parameter)
        {


            LoadedAnalyticalRuns = _fileDataService.LoadProjects();
        }

        private bool CanOpenAnalyticalRun(object parameter)
        {
            return !string.IsNullOrEmpty(NewProjectName)
                && NewProjectName.Length <= MAXPROJECTNAMELENGTH
                && !SelectedProjectAlreadyExists()
                && !SelectedProjectContainsIllegalCharacter();
        }

        private bool SelectedProjectAlreadyExists()
        {
            var existingProjectNames = LoadedAnalyticalRuns.Select(p => p.ProjectID);
            return existingProjectNames.Contains(NewProjectName);
        }

        private bool SelectedProjectContainsIllegalCharacter()
        {
            var illegalCharactersPattern = "[<>\\/\":|?*.]+";
            return Regex.Match(NewProjectName, illegalCharactersPattern).Success;
        }


        private void DeleteAnalyticalRun(object parameter)
        {
            _fileDataService.DeleteProject(SelectedAnalyticalRun);
            LoadedAnalyticalRuns = _fileDataService.LoadProjects();
            SelectedAnalyticalRun = null;
        }

        private bool CanDeleteAnalyticalRun(object parameter)
        {
            return SelectedAnalyticalRun is not null;
        }

    }
}
