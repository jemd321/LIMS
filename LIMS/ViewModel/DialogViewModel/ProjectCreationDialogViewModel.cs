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
        private int _selectedProjectName;
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

        public int SelectedProjectName
        {
            get { return _selectedProjectName; }
            set { _selectedProjectName = value; RaisePropertyChanged(); }
        }

        public new void Load()
        {
            LoadedProjects = _fileDataService.LoadProjects();
        }

        private void CreateProject(object parameter)
        {

        }

        private void DeleteProject(object parameter)
        {
            throw new NotImplementedException();
        }

    }
}