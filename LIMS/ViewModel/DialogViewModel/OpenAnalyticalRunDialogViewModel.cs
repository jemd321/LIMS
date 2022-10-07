using System;
using System.Collections.ObjectModel;
using System.Linq;
using LIMS.Command;
using LIMS.Data;
using LIMS.Model;

namespace LIMS.ViewModel.DialogViewModel
{
    /// <summary>
    /// ViewModel for the open analytical run dialog, allowing user to choose a run to open.
    /// </summary>
    public class OpenAnalyticalRunDialogViewModel : ViewModelBase, IStringIODialogViewModel
    {
        private readonly IDataService _dataService;
        private ObservableCollection<string> _loadedAnalyticalRunIDs = new();
        private string _selectedAnalyticalRun;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenAnalyticalRunDialogViewModel"/> class.
        /// </summary>
        /// <param name="dataService">A reference to the dataService that the dialog will use to load a list of runs that the user may open.</param>
        public OpenAnalyticalRunDialogViewModel(IDataService dataService)
        {
            _dataService = dataService;

            OpenAnalyticalRunCommand = new DelegateCommand(OpenAnalyticalRun, CanOpenAnalyticalRun);
            DeleteAnalyticalRunCommand = new DelegateCommand(DeleteAnalyticalRun, CanDeleteAnalyticalRun);
        }

        /// <inheritdoc/>
        public event EventHandler DialogAccepted;

        /// <summary>
        /// Gets the command executed when the user presses the open button with a run selected.
        /// </summary>
        public DelegateCommand OpenAnalyticalRunCommand { get; }

        /// <summary>
        /// Gets the command executed when the user presses the delete button with a run selected.
        /// </summary>
        public DelegateCommand DeleteAnalyticalRunCommand { get; }

        /// <summary>
        /// Gets or sets the observable collection of runs that exist in the data service.
        /// </summary>
        public ObservableCollection<string> LoadedAnalyticalRunIDs
        {
            get => _loadedAnalyticalRunIDs;
            set
            {
                _loadedAnalyticalRunIDs = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the dialog input, in this case this should be the projectID from which to load the runs.
        /// </summary>
        public string DialogInput { get; set; }

        /// <summary>
        /// Gets or sets the dialog output, in this case the ID of the run that the user has selected to open.
        /// </summary>
        public string DialogOutput { get; set; }

        /// <summary>
        /// Gets the ID of the currently open project that was supplied to the dialog.
        /// </summary>
        public string OpenProjectID { get; private set; }

        /// <summary>
        /// Gets the Project object that was created to represent the currently open project.
        /// </summary>
        public Project OpenProject { get; private set; }

        /// <summary>
        /// Gets or sets the currently selected analytical run.
        /// </summary>
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

        /// <summary>
        /// Initializes the dialog.
        /// </summary>
        public override void Load()
        {
            OpenProjectID = DialogInput;
            var openProject = _dataService
                .LoadProjects()
                .Where(p => p.ProjectID == OpenProjectID)
                .SingleOrDefault();
            OpenProject = openProject;
            LoadAnalyticalRuns();
        }

        /// <summary>
        /// Raises the dialog accepted event, indicating that the user wishes to open a run.
        /// </summary>
        /// <param name="e">Event args - empty.</param>
        protected virtual void RaiseDialogAccepted(EventArgs e)
        {
            DialogAccepted?.Invoke(this, e);
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

        private bool CanOpenAnalyticalRun(object parameter)
        {
            return !string.IsNullOrEmpty(SelectedAnalyticalRun);
        }

        private void DeleteAnalyticalRun(object parameter)
        {
            _dataService.DeleteAnalyticalRun(OpenProject, SelectedAnalyticalRun);
            LoadedAnalyticalRunIDs.Remove(SelectedAnalyticalRun);
        }

        private bool CanDeleteAnalyticalRun(object parameter)
        {
            return SelectedAnalyticalRun is not null;
        }
    }
}
