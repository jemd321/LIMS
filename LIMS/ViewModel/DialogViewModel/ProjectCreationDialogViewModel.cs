using LIMS.Command;
using LIMS.Data;
using LIMS.Model;
using System;
using System.Collections.ObjectModel;

namespace LIMS.ViewModel
{
    public class ProjectCreationDialogViewModel : ViewModelBase
    {
        private readonly IFileDataService _fileDataService;
        private string _selectedProjectName;
        private ObservableCollection<Project> _loadedProjects;

        public ProjectCreationDialogViewModel(IFileDataService fileDataService)
        {
            _fileDataService = fileDataService;

            CreateProjectCommand = new DelegateCommand(CreateProject);
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
            set { _selectedProjectName = value; RaisePropertyChanged(); }
        }

        public override void Load()
        {
            LoadedProjects = _fileDataService.LoadProjects();
        }

        private void CreateProject(object parameter)
        {
            var newProject = new Project(SelectedProjectName);
            _fileDataService.CreateProject(newProject);
        }

        private void DeleteProject(object parameter)
        {
            throw new NotImplementedException();
        }

    }
}