using System;
using System.Collections.ObjectModel;
using System.Linq;
using LIMS.Command;
using LIMS.Data;
using LIMS.Model;

namespace LIMS.ViewModel.DialogViewModel
{
    public class OpenAnalyticalRunDialogViewModel : ViewModelBase, IStringIODialogViewModel
    {
        private readonly IDataService _fileDataService;
        private ObservableCollection<string> _loadedAnalyticalRunIDs = new();
        private string _selectedAnalyticalRun;

        public event EventHandler DialogAccepted;

        public OpenAnalyticalRunDialogViewModel(IDataService fileDataService)
        {
            _fileDataService = fileDataService;

            OpenAnalyticalRunCommand = new DelegateCommand(OpenAnalyticalRun, CanOpenAnalyticalRun);
            DeleteAnalyticalRunCommand = new DelegateCommand(DeleteAnalyticalRun, CanDeleteAnalyticalRun);
        }

        public DelegateCommand OpenAnalyticalRunCommand { get; }

        public DelegateCommand DeleteAnalyticalRunCommand { get; }

        public ObservableCollection<string> LoadedAnalyticalRunIDs
        {
            get => _loadedAnalyticalRunIDs;
            set { _loadedAnalyticalRunIDs = value; RaisePropertyChanged(); }
        }

        // In this dialog, the calling viewModel should supply the currently open ProjectID
        public string DialogInput { get; set; }

        public string DialogOutput { get; set; }

        public string OpenProjectID { get; private set; }

        public Project OpenProject { get; private set; }

        public string SelectedAnalyticalRun
        {
            get => _selectedAnalyticalRun;
            set
            {
                _selectedAnalyticalRun = value;
                RaisePropertyChanged();
                OpenAnalyticalRunCommand.RaiseCanExecuteChanged();
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
            LoadAnalyticalRuns();
        }

        private void LoadAnalyticalRuns()
        {
            foreach (string analyticalRunID in OpenProject.AnalyticalRunIDs)
            {
                LoadedAnalyticalRunIDs.Add(analyticalRunID);
            }
        }

        private void OpenAnalyticalRun(object parameter)
        {
            if (SelectedAnalyticalRun is null)
            {
                return;
            }

            DialogOutput = SelectedAnalyticalRun;
            RaiseDialogAccepted(EventArgs.Empty);
        }

        protected virtual void RaiseDialogAccepted(EventArgs e)
        {
            DialogAccepted?.Invoke(this, e);
        }

        private bool CanOpenAnalyticalRun(object parameter)
        {
            return !string.IsNullOrEmpty(SelectedAnalyticalRun);
        }

        private void DeleteAnalyticalRun(object parameter)
        {
            _fileDataService.DeleteAnalyticalRun(OpenProject, SelectedAnalyticalRun);
            LoadedAnalyticalRunIDs.Remove(SelectedAnalyticalRun);
        }

        private bool CanDeleteAnalyticalRun(object parameter)
        {
            return SelectedAnalyticalRun is not null;
        }
    }
}
