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
        void SetupApplicationStorage();
        List<Project> LoadProjects();
        string LoadRun(AnalyticalRun analyticalRun);
        void SaveAnalyticalRun(AnalyticalRun analyticalRun);
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

        public void SetupApplicationStorage()
        {
            if (IsApplicationStorageSetup())
            {
                return;
            }
            if (!_fileSystem.Directory.Exists(ApplicationDirectory))
            {
                _fileSystem.Directory.CreateDirectory(ApplicationDirectory);
            }
            if (!_fileSystem.Directory.Exists(ProjectsDirectory))
            {
                _fileSystem.Directory.CreateDirectory(ProjectsDirectory);
            }
        }

        private bool IsApplicationStorageSetup()
        {
            if (ApplicationDirectory == string.Empty || ProjectsDirectory == string.Empty)
            {
                return false;
            }
            if (_fileSystem.Directory.Exists(ApplicationDirectory) && _fileSystem.Directory.Exists(ProjectsDirectory))
            {
                return true;
            }
            else return false;
        }

        public List<Project> LoadProjects()
        {
            var projects = new List<Project>();
            string[] projectDirectories = _fileSystem.Directory.GetDirectories(ProjectsDirectory);
            foreach (var projectDirectory in projectDirectories)
            {
                string projectID = projectDirectory.Split('\\').Last();
                var project = new Project(projectID);
                string[] analyticalRunDirectories = _fileSystem.Directory.GetDirectories(projectDirectory);
                foreach (var analyticalRunDirectory in analyticalRunDirectories)
                {
                    string analyticalRunID = analyticalRunDirectory.Split('\\').Last();
                    project.AnalyticalRunIDs.Add(analyticalRunID);
                }
                projects.Add(project);
            }
            return projects;
        }

        public string LoadRun(AnalyticalRun analyticalRun)
        {
            throw new NotImplementedException();
        }

        public void SaveAnalyticalRun(AnalyticalRun analyticalRun)
        {
            var projects = from p in _fileSystem.Directory.GetDirectories(ProjectsDirectory)
                           select p.Split('\\').Last();
            if (!projects.Contains(analyticalRun.ParentProjectID))
            {
                throw new DirectoryNotFoundException("No project exists for this analytical run");
            }

            string projectDirectory = _fileSystem.Path.Combine(ProjectsDirectory, analyticalRun.ParentProjectID);
            string filePath = _fileSystem.Path.Combine(projectDirectory, analyticalRun.AnalyticalRunID + ".Json");

            string jsonDoc = JsonSerializer.Serialize(analyticalRun.RegressionData);
            _fileSystem.File.WriteAllText(filePath, jsonDoc);
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
