using LIMS.Model;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO.Abstractions;

namespace LIMS.Data
{
    public interface IFileDataService
    {
        string ApplicationDirectory { get; }
        string ProjectsDirectory { get; }
        void CreateApplicationStorage();
        bool IsApplicationStorageSetup();
        List<Project> LoadProjects();
        string LoadRun(AnalyticalRun analyticalRun);
        void SaveRun(RegressionData regressionData);
        FileInfo ValidateFilePath(string filePath);
        Task<string> GetRawData(FileInfo fileInfo);
    }

    public class FileDataService : IFileDataService
    {
        private readonly IFileSystem _fileSystem;

        public FileDataService(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public string ApplicationDirectory => GetApplicationDirectory();
        public string ProjectsDirectory => GetProjectsDirectory();

        private string GetApplicationDirectory()
        {
            const string APPLICATIONDIRECTORYNAME = "LIMS";
            var appDataRoaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            // Application Data Roaming not found
            if (string.IsNullOrEmpty(appDataRoaming))
            {
                return string.Empty;
            }

            return Path.Combine(appDataRoaming, APPLICATIONDIRECTORYNAME);
        }

        private string GetProjectsDirectory()
        {
            const string PROJECTSDIRECTORYNAME = "Projects";
            if (ApplicationDirectory == string.Empty)
            {
                return string.Empty;
            }
            return Path.Combine(ApplicationDirectory, PROJECTSDIRECTORYNAME);
        }

        public void CreateApplicationStorage()
        {
            if (IsApplicationStorageSetup())
            {
                return;
            }
            if (!Directory.Exists(ApplicationDirectory))
            {
                Directory.CreateDirectory(ApplicationDirectory);
            }
            if (!Directory.Exists(ProjectsDirectory))
            {
                Directory.CreateDirectory(ProjectsDirectory);
            }
        }

        public bool IsApplicationStorageSetup()
        {
            if (ApplicationDirectory == string.Empty || ProjectsDirectory == string.Empty)
            {
                return false;
            }
            if (Directory.Exists(ApplicationDirectory) && Directory.Exists(ProjectsDirectory))
            {
                return true;
            }
            else return false;
        }

        public List<Project> LoadProjects()
        {
            if (!IsApplicationStorageSetup())
            {
                return null;
            }
            var projects = new List<Project>();
            string[] projectDirectories = Directory.GetDirectories(ProjectsDirectory);
            foreach (var projectDirectory in projectDirectories)
            {
                var project = new Project(projectDirectory);
                string[] analyticalRunDirectories = Directory.GetDirectories(projectDirectory);
                foreach (var analyticalRunDirectory in analyticalRunDirectories)
                {
                    var analyticalRun = new AnalyticalRun(analyticalRunDirectory, project.ProjectID);
                    project.AnalyticalRuns.Add(analyticalRun.AnalyticalRunID, analyticalRun);
                }
                projects.Add(project);
            }
            return projects;
        }

        public string LoadRun(AnalyticalRun analyticalRun)
        {
            throw new NotImplementedException();
        }

        public void SaveRun(RegressionData regressionData)
        {
            var jsonDoc = JsonSerializer.Serialize<RegressionData>(regressionData);

        }

        // TODO add file validation
        public FileInfo ValidateFilePath(string filePath)
        {
            return new FileInfo(filePath);
        }

        public Task<string> GetRawData(FileInfo fileInfo)
        {
            return Task.Run(() => File.ReadAllText(fileInfo.FullName));
        }
    }
}
