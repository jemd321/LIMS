using LIMS.Command;
using LIMS.Data;
using LIMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LIMS.ViewModel.DialogViewModel
{
    public class SaveAnalyticalRunDialogViewModel : ValidationViewModelBase, IStringIODialogViewModel
    {
        const int MAXANALYTICALRUNIDLENGTH = 36;
        private string _chosenAnalyticalRunID;
        private readonly IDataProvider _fileDataProvider;

        public SaveAnalyticalRunDialogViewModel(IDataProvider fileDataService)
        {
            _fileDataProvider = fileDataService;

            SaveAnalyticalRunCommand = new DelegateCommand(SaveAnalyticalRun, CanSaveAnalyticalRun);
        }

        public DelegateCommand SaveAnalyticalRunCommand { get; }
        public string DialogInput { get; set; }
        public string DialogOutput { get; set; }
        public string OpenProjectID { get; private set; }
        public Project OpenProject { get; private set; }

        public event EventHandler DialogAccepted;

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

        public string ChosenAnalyticalRunID
        {
            get { return _chosenAnalyticalRunID; }
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

        private readonly List<string> LoadedAnalyticalRunIDs = new();

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

        private void LoadAnalyticalRuns()
        {
            foreach (string analyticalRunID in OpenProject.AnalyticalRunIDs)
            {
                LoadedAnalyticalRunIDs.Add(analyticalRunID);
            }
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

        private bool SelectedAnalyticalRunIDAlreadyExists(string chosenAnalyticalRunID)
        {
            return LoadedAnalyticalRunIDs.Contains(chosenAnalyticalRunID); 
        }

    }
}
