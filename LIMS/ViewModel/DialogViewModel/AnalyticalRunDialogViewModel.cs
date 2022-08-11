using LIMS.Command;
using LIMS.Data;
using LIMS.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace LIMS.ViewModel.DialogViewModel
{
    public class AnalyticalRunDialogViewModel : ValidationViewModelBase, IStringIODialogViewModel
    {
        private const int MAXPROJECTNAMELENGTH = 36;

        private readonly IFileDataService _fileDataService;
        private ObservableCollection<string> _loadedAnalyticalRunIDs;
        private string _newProjectName;
        private string _selectedAnalyticalRun;

        public AnalyticalRunDialogViewModel(IFileDataService fileDataService)
        {
            _fileDataService = fileDataService;

            OpenAnalyticalRunCommand = new DelegateCommand(OpenAnalyticalRun, CanOpenAnalyticalRun);
            DeleteAnalyticalRunCommand = new DelegateCommand(DeleteAnalyticalRun, CanDeleteAnalyticalRun);
        }

        public DelegateCommand OpenAnalyticalRunCommand { get; }
        public DelegateCommand DeleteAnalyticalRunCommand { get; }

        public ObservableCollection<string> LoadedAnalyticalRunIDs
        {
            get { return _loadedAnalyticalRunIDs; }
            set { _loadedAnalyticalRunIDs = value; RaisePropertyChanged(); }
        }

        // In this dialog, the calling viewModel should supply the currently open ProjectID
        public string DialogInput { get; set; }
        public string DialogOutput { get; set; }
        public string OpenProjectID { get; private set; }
        public Project OpenProject { get; private set; }
        public string SelectedAnalyticalRun
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
            OpenProjectID = DialogInput;
            var openProject = _fileDataService
                .LoadProjects()
                .Where(p => p.ProjectID == OpenProjectID)
                .SingleOrDefault();
            OpenProject = openProject;
            foreach (string analyticalRunID in OpenProject.AnalyticalRunIDs)
            {
                LoadedAnalyticalRunIDs.Add(analyticalRunID);
            }
        }

        private void OpenAnalyticalRun(object parameter)
        {


        }

        private bool CanOpenAnalyticalRun(object parameter)
        {
            return true;
        }

        private bool SelectedProjectAlreadyExists()
        {
            return true;
        }

        private bool SelectedProjectContainsIllegalCharacter()
        {
            return true;
        }


        private void DeleteAnalyticalRun(object parameter)
        {
            throw new NotImplementedException();
        }

        private bool CanDeleteAnalyticalRun(object parameter)
        {
            return SelectedAnalyticalRun is not null;
        }

    }
}
