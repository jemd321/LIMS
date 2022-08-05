using LIMS.Command;
using LIMS.Data;
using LIMS.Model;
using System;
using System.Collections.ObjectModel;

namespace LIMS.ViewModel
{
    public class ProjectCreationDialogViewModel : ValidationViewModelBase
    {
        private readonly IFileDataService _fileDataService;
        private ObservableCollection<Project> _loadedProjects;
        private string _selectedProjectName;

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

        public string SelectedProjectName
        {
            get { return _selectedProjectName; }
            set
            {
                _selectedProjectName = value;
                RaisePropertyChanged();
                CreateProjectCommand.RaiseCanExecuteChanged();
                if (string.IsNullOrEmpty(_selectedProjectName))
                {
                    AddError("Project Name is required");
                }
                else
                {
                    ClearErrors();
                }
            }
        }

        public override void Load()
        {
            LoadedProjects = _fileDataService.LoadProjects();
        }

        private void CreateProject(object parameter)
        {
            var newProject = new Project(SelectedProjectName);
            _fileDataService.CreateProject(newProject);
            LoadedProjects = _fileDataService.LoadProjects();
        }

        private bool CanCreateProject(object parameter) => !string.IsNullOrEmpty(SelectedProjectName);

        private void DeleteProject(object parameter)
        {
            throw new NotImplementedException();
        }

    }
}