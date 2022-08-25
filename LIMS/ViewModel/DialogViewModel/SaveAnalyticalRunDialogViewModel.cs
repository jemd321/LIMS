using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LIMS.Command;
using LIMS.Data;
using LIMS.Model;

namespace LIMS.ViewModel.DialogViewModel
{
    public class SaveAnalyticalRunDialogViewModel : ValidationViewModelBase, IStringIODialogViewModel
    {
        private const int MAXANALYTICALRUNIDLENGTH = 36;
        private readonly IDataProvider _fileDataProvider;
        private readonly List<string> _loadedAnalyticalRunIDs = new();
        private string _chosenAnalyticalRunID;

        public SaveAnalyticalRunDialogViewModel(IDataProvider fileDataService)
        {
            _fileDataProvider = fileDataService;

            SaveAnalyticalRunCommand = new DelegateCommand(SaveAnalyticalRun, CanSaveAnalyticalRun);
        }

        public event EventHandler DialogAccepted;

        public DelegateCommand SaveAnalyticalRunCommand { get; }

        public string DialogInput { get; set; }

        public string DialogOutput { get; set; }

        public string OpenProjectID { get; private set; }

        public Project OpenProject { get; private set; }

        public string ChosenAnalyticalRunID
        {
            get => _chosenAnalyticalRunID;
            set
            {
                _chosenAnalyticalRunID = value;
                RaisePropertyChanged();

                ClearErrors();
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

        public override void Load()
        {
            OpenProjectID = DialogInput;
            var openProject = _fileDataProvider
                .LoadProjects()
                .Where(p => p.ProjectID == OpenProjectID)
                .SingleOrDefault();
            OpenProject = openProject;
            LoadAnalyticalRuns();
        }

        protected virtual void RaiseDialogAccepted(EventArgs e)
        {
            DialogAccepted?.Invoke(this, e);
        }

        private static bool SelectedAnalyticalRunIDContainsIllegalCharacter(string chosenAnalyticalRunID)
        {
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

            // Validation
        }

        private bool SelectedAnalyticalRunIDAlreadyExists(string chosenAnalyticalRunID)
        {
            return _loadedAnalyticalRunIDs.Contains(chosenAnalyticalRunID);
        }
    }
}
