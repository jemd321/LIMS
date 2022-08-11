﻿using LIMS.Model;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO.Abstractions;
using System.Collections.ObjectModel;

namespace LIMS.Data
{
    public interface IFileDataService
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

        private static string GetApplicationDirectory()
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

        public void CreateProject(Project newProject)
        {
            string newProjectDirectory = _fileSystem.Path.Combine(ProjectsDirectory, newProject.ProjectID);
            if (_fileSystem.Directory.Exists(newProjectDirectory))
            {
                return;
            }
            _fileSystem.Directory.CreateDirectory(newProjectDirectory);
        }
        public void DeleteProject(Project existingProject)
        {
            string existingProjectDirectory = _fileSystem.Path.Combine(ProjectsDirectory, existingProject.ProjectID);
            if (_fileSystem.Directory.Exists(existingProjectDirectory))
            {
                _fileSystem.Directory.Delete(existingProjectDirectory, true);
            }
            else return;
        }

        public ObservableCollection<Project> LoadProjects()
        {
            var projects = new ObservableCollection<Project>();
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

        public AnalyticalRun LoadAnalyticalRun(Project project, string analyticalRunID)
        {
            string analyticalRunDirectory = _fileSystem.Path.Combine(ProjectsDirectory, project.ProjectID, analyticalRunID);
            string runFileName = analyticalRunID + ".json";
            string runFilePath = _fileSystem.Path.Combine(analyticalRunDirectory, runFileName);
            if (!_fileSystem.File.Exists(runFilePath))
            {
                // File not found exception - if the file does not exist then the project/analytical run list is bugged.
                throw new FileNotFoundException("No analytical run file was found for the selected analytical run ID");
            }
            string fileContents = _fileSystem.File.ReadAllText(runFilePath);
            var regressionData = JsonSerializer.Deserialize<RegressionData>(fileContents);
            return new AnalyticalRun(analyticalRunID, project.ProjectID, regressionData);
        }

        public void SaveAnalyticalRun(AnalyticalRun analyticalRun)
        {
            var projects = from p in _fileSystem.Directory.GetDirectories(ProjectsDirectory)
                           select p.Split('\\').Last();
            if (!projects.Contains(analyticalRun.ParentProjectID))
            {
                throw new DirectoryNotFoundException("No project exists for this analytical run");
            }

            string analyticalRunDirectory = _fileSystem.Path.Combine(ProjectsDirectory, analyticalRun.ParentProjectID, analyticalRun.AnalyticalRunID);
            string filePath = _fileSystem.Path.Combine(analyticalRunDirectory, analyticalRun.AnalyticalRunID + ".json");

            string jsonDoc = JsonSerializer.Serialize(analyticalRun.RegressionData);
            _fileSystem.File.WriteAllText(filePath, jsonDoc);
        }

        public void DeleteAnalyticalRun(Project project, string analyticalRunID)
        {
            string analyticalRunDirectory = _fileSystem.Path.Combine(ProjectsDirectory, project.ProjectID, analyticalRunID);
            if (_fileSystem.Directory.Exists(analyticalRunDirectory))
            {
                _fileSystem.Directory.Delete(analyticalRunDirectory, true);
            }
            else return;
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
