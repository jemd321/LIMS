using System.Collections.ObjectModel;
using System.IO;
using LIMS.Model;

namespace LIMS.Data
{
    public interface IDataProvider
    {
        string ApplicationDirectory { get; }

        string ProjectsDirectory { get; }

        ObservableCollection<Project> LoadProjects();

        void SetupApplicationStorage();

        void CreateProject(Project newProject);

        void DeleteProject(Project existingProject);

        AnalyticalRun LoadAnalyticalRun(Project project, string analyticalRunID);

        void DeleteAnalyticalRun(Project project, string analyticalRunID);

        void SaveAnalyticalRun(AnalyticalRun analyticalRun);

        FileInfo ValidateFilePath(string filePath);

        string GetRawData(FileInfo fileInfo);
    }
}
