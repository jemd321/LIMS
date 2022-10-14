using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LIMS.Command;
using LIMS.Data;
using LIMS.Model;

namespace LIMS.ViewModel.DialogViewModel
{
    /// <summary>
    /// ViewModel for the save analytical run dialog, responsible for allowing the user to select a name for a newly created run.
    /// </summary>
    public class SaveAnalyticalRunDialogViewModel : ValidationViewModelBase, IStringIODialogViewModel
    {
        private readonly IDataService _dataService;
        private readonly List<string> _loadedAnalyticalRunIDs = new();
        private string _chosenAnalyticalRunID;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveAnalyticalRunDialogViewModel"/> class.
        /// </summary>
        /// <param name="dataService">The data service that handles saving the analytical run.</param>
        public SaveAnalyticalRunDialogViewModel(IDataService dataService)
        {
            _dataService = dataService;

            SaveAnalyticalRunCommand = new DelegateCommand(SaveAnalyticalRun, CanSaveAnalyticalRun);
        }

        /// <summary>
        /// Event that signals the user has accepted the dialog rather than simply closed it.
        /// </summary>
        public event EventHandler DialogAccepted;

        /// <summary>
        /// Gets the command that saves the analytical run when the user presses the save button.
        /// </summary>
        public DelegateCommand SaveAnalyticalRunCommand { get; }

        /// <summary>
        /// Gets or sets the string that was supplied to the dialog as an input, in this case the ID of the currently open project.
        /// </summary>
        public string DialogInput { get; set; }

        /// <summary>
        /// Gets or sets the string that will be the result of the dialog, in this case the ID of the analytical run name the user has chosen.
        /// </summary>
        public string DialogOutput { get; set; }

        /// <summary>
        /// Gets the ID of the currently open project as a string.
        /// </summary>
        public string OpenProjectID { get; private set; }

        /// <summary>
        /// Gets the currently open project.
        /// </summary>
        public Project OpenProject { get; private set; }

        /// <summary>
        /// Gets or sets the ID of the analytical run that the user enters in the UI.
        /// </summary>
        public string ChosenAnalyticalRunID
        {
            get => _chosenAnalyticalRunID;
            set
            {
                _chosenAnalyticalRunID = value;
                RaisePropertyChanged();

                ClearErrors();

                // Length set to 36 chars to avoid the resulting file name being too long for Windows.
                const int MAXANALYTICALRUNIDLENGTH = 36;
                if (ChosenAnalyticalRunID.Length > MAXANALYTICALRUNIDLENGTH)
                {
                    AddError("Analytical run name is too long");
                }
                else if (SelectedAnalyticalRunIDAlreadyExists(value))
                {
                    AddError("Project already exists");
                }
                else if (SelectedAnalyticalRunIDContainsIllegalCharacter(value))
                {
                    AddError("Project name cannot contain: < > \\ / \" : | ? * .");
                }
                else
                {
                    ClearErrors();
                }

                SaveAnalyticalRunCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Initialises the dialog.
        /// </summary>
        public override void Load()
        {
            // This dialog is supplied with the current project ID as an input.
            OpenProjectID = DialogInput;
            var openProject = _dataService
                .LoadProjects()
                .Where(p => p.ProjectID == OpenProjectID)
                .SingleOrDefault();
            OpenProject = openProject;
            LoadAnalyticalRuns();
        }

        /// <summary>
        /// Raises the dialog accepted event, indicating that the user has chosen a run name rather than cancelling the dialog.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void RaiseDialogAccepted(EventArgs e)
        {
            DialogAccepted?.Invoke(this, e);
        }

        private static bool SelectedAnalyticalRunIDContainsIllegalCharacter(string chosenAnalyticalRunID)
        {
            // Illegal characters for Windows file names.
            var illegalCharactersPattern = @"[\\<>/"":|?*.]+";
            return Regex.Match(chosenAnalyticalRunID, illegalCharactersPattern).Success;
        }

        private void LoadAnalyticalRuns()
        {
            foreach (string analyticalRunID in OpenProject.AnalyticalRunIDs)
            {
                _loadedAnalyticalRunIDs.Add(analyticalRunID);
            }
        }

        private bool CanSaveAnalyticalRun(object parameter)
        {
            return !string.IsNullOrEmpty(ChosenAnalyticalRunID);
        }

        private void SaveAnalyticalRun(object obj)
        {
            DialogOutput = ChosenAnalyticalRunID;
            RaiseDialogAccepted(EventArgs.Empty);
        }

        private bool SelectedAnalyticalRunIDAlreadyExists(string chosenAnalyticalRunID)
        {
            return _loadedAnalyticalRunIDs.Contains(chosenAnalyticalRunID);
        }
    }
}
