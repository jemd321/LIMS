using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using LIMS.Command;
using LIMS.Data;
using LIMS.Model;

namespace LIMS.ViewModel.DialogViewModel
{
    /// <summary>
    /// ViewModel for the dialog that allows users to create and delete projects.
    /// </summary>
    public class ProjectEditDialogViewModel : ValidationViewModelBase, IDialogViewModel
    {
        private readonly IDataService _dataService;
        private ObservableCollection<Project> _loadedProjects;
        private string _newProjectName;
        private Project _selectedProject;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectEditDialogViewModel"/> class.
        /// </summary>
        /// <param name="dataService">The data service that will handle the creation and deletion of projects in storage.</param>
        public ProjectEditDialogViewModel(IDataService dataService)
        {
            _dataService = dataService;

            CreateProjectCommand = new DelegateCommand(CreateProject, CanCreateProject);
            DeleteProjectCommand = new DelegateCommand(DeleteProject, CanDeleteProject);
        }

        /// <inheritdoc/>
        public event EventHandler DialogAccepted;

        /// <summary>
        /// Gets the command that executes when the user clicks the create project button, creating the new project in storage.
        /// </summary>
        public DelegateCommand CreateProjectCommand { get; }

        /// <summary>
        /// Gets the command that executes when the user clicks the delete project button, deleting the project from storage.
        /// </summary>
        public DelegateCommand DeleteProjectCommand { get; }

        /// <summary>
        /// Gets or sets the collection of projects that has been loaded from storage.
        /// </summary>
        public ObservableCollection<Project> LoadedProjects
        {
            get => _loadedProjects;
            set
            {
                _loadedProjects = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the name of the project that the user has selected.
        /// </summary>
        public string NewProjectName
        {
            get => _newProjectName;
            set
            {
                // convert to lower case to avoid problems with case sensitive directory names
                _newProjectName = value.ToLower();
                RaisePropertyChanged();

                ClearErrors();

                // Length set to 36 chars to avoid the resulting file name being too long for Windows.
                const int MAXPROJECTNAMELENGTH = 36;
                if (NewProjectName.Length > MAXPROJECTNAMELENGTH)
                {
                    AddError("Project name is too long");
                }
                else if (SelectedProjectAlreadyExists())
                {
                    AddError("Project already exists");
                }
                else if (SelectedProjectContainsIllegalCharacter())
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

        /// <summary>
        /// Gets or sets the project that the user has selected in the dialog.
        /// </summary>
        public Project SelectedProject
        {
            get => _selectedProject;
            set
            {
                _selectedProject = value;
                RaisePropertyChanged();
                DeleteProjectCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Initialises the viewModel.
        /// </summary>
        public override void Load()
        {
            LoadedProjects = _dataService.LoadProjects();
            NewProjectName = string.Empty;
        }

        private void CreateProject(object parameter)
        {
            var newProject = new Project(NewProjectName);
            _dataService.CreateProject(newProject);
            LoadedProjects = _dataService.LoadProjects();
        }

        private bool CanCreateProject(object parameter)
        {
            // Length set to 36 chars to avoid the resulting file name being too long for Windows.
            const int MAXPROJECTNAMELENGTH = 36;
            return !string.IsNullOrEmpty(NewProjectName)
                && NewProjectName.Length <= MAXPROJECTNAMELENGTH
                && !SelectedProjectAlreadyExists()
                && !SelectedProjectContainsIllegalCharacter();
        }

        private bool SelectedProjectAlreadyExists()
        {
            var existingProjectNames = LoadedProjects.Select(p => p.ProjectID);
            return existingProjectNames.Contains(NewProjectName);
        }

        private bool SelectedProjectContainsIllegalCharacter()
        {
            var illegalCharactersPattern = @"[\\<>/"":|?*.]+";
            return Regex.Match(NewProjectName, illegalCharactersPattern).Success;
        }

        private void DeleteProject(object parameter)
        {
            _dataService.DeleteProject(SelectedProject);
            LoadedProjects = _dataService.LoadProjects();
            SelectedProject = null;
        }

        private bool CanDeleteProject(object parameter)
        {
            return SelectedProject is not null;
        }
    }
}