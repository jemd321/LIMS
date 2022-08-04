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

        public ProjectCreationDialogViewModel(IFileDataService fileDataService)
        {
            _fileDataService = fileDataService;

            CreateProjectCommand = new DelegateCommand(CreateProject);
            DeleteProjectCommand = new DelegateCommand(DeleteProject);
        }


        public DelegateCommand CreateProjectCommand { get; }
        public DelegateCommand DeleteProjectCommand { get; }

        public ObservableCollection<Project> LoadedProjects { get; private set; }

        public int SelectedProjectName
        {
            get { return _selectedProjectName; }
            set { _selectedProjectName = value; RaisePropertyChanged(); }
        }
        private void CreateProject(object parameter)
        {
            LoadedProjects = _fileDataService.LoadProjects();
        }

        private void DeleteProject(object parameter)
        {
            throw new NotImplementedException();
        }

    }
}