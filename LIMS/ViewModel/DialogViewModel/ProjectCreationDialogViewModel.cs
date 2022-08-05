using LIMS.Command;
using LIMS.Data;
using LIMS.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace LIMS.ViewModel
{
    public class ProjectCreationDialogViewModel : ValidationViewModelBase
    {
        private readonly IFileDataService _fileDataService;
        private ObservableCollection<Project> _loadedProjects;
        private string _newProjectName;

        public ProjectCreationDialogViewModel(IFileDataService fileDataService)
        {
            _fileDataService = fileDataService;

            CreateProjectCommand = new DelegateCommand(CreateProject, CanCreateProject);
            DeleteProjectCommand = new DelegateCommand(DeleteProject);
        }

        public DelegateCommand CreateProjectCommand { get; }
        public DelegateCommand DeleteProjectCommand { get; }

        public ObservableCollection<Project> LoadedProjects
        {
            get { return _loadedProjects; }
            set { _loadedProjects = value; RaisePropertyChanged(); }
        }

        public string NewProjectName
        {
            get { return _newProjectName; }
            set
            {
                _newProjectName = value.ToLower();
                RaisePropertyChanged();

                ClearErrors();
                if (DoesSelectedProjectAlreadyExist())
                {
                    AddError("Project already exists");
                }
                else if (DoesSelectedProjectContainIllegalCharacter())
                {
                    AddError("Project name cannot contain: < > \\ / \" : | ? * .");
                }
                else
                {
                    ClearErrors();
                }
                CreateProjectCommand.RaiseCanExecuteChanged();
            }
        }

        public override void Load()
        {
            LoadedProjects = _fileDataService.LoadProjects();
            NewProjectName = "";
        }

        private void CreateProject(object parameter)
        {
            var newProject = new Project(NewProjectName);
            _fileDataService.CreateProject(newProject);
            LoadedProjects = _fileDataService.LoadProjects();
        }

        private bool DoesSelectedProjectAlreadyExist()
        {
            var existingProjectNames = LoadedProjects.Select(p => p.ProjectID);
            return existingProjectNames.Contains(NewProjectName);
        }

        private bool DoesSelectedProjectContainIllegalCharacter()
        {
            var illegalCharactersPattern = "[<>\\/\":|?*.]+";
            return Regex.Match(NewProjectName, illegalCharactersPattern).Success;
        }

        private bool CanCreateProject(object parameter)
        {
            return !string.IsNullOrEmpty(NewProjectName) && !DoesSelectedProjectAlreadyExist() && !DoesSelectedProjectContainIllegalCharacter();
        }

        private void DeleteProject(object parameter)
        {
            throw new NotImplementedException();
        }

    }
}