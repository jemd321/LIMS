using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text.Json;
using LIMS.Enums;
using LIMS.Model;

namespace LIMS.Data
{
    /// <summary>
    /// Provides CRUD services for storage of projects, runs and regression data on the local filesystem.
    /// </summary>
    public class FileDataService : IDataService
    {
        private readonly IFileSystem _fileSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDataService"/> class.
        /// </summary>
        /// <param name="fileSystem">IO.Abstractions wrapper of the Windows file system.</param>
        public FileDataService(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        private static string ApplicationDirectory => GetApplicationDirectory();

        private static string ProjectsDirectory => GetProjectsDirectory();

        /// <summary>
        /// Reads raw data from a text file, indicating whether or not the operation succeeded.
        /// </summary>
        /// <param name="filePath">the filePath of the text file to be read.</param>
        /// <returns>A <see cref="DataReadResult"/> object containing the raw data as a string, and the success/fail state of the operation.</returns>
        /// <remarks>This method would typically be used to read exports from external instrument data acquisition or processing software.</remarks>
        public DataReadResult ReadDataFromTextFile(string filePath)
        {
            try
            {
                string rawData = File.ReadAllText(filePath);
                return new DataReadResult
                {
                    Data = rawData,
                    DataReadFailureReason = DataReadFailureReason.None,
                };
            }
            catch (PathTooLongException)
            {
                return new DataReadResult { DataReadFailureReason = DataReadFailureReason.PathTooLong };
            }
            catch (DirectoryNotFoundException)
            {
                return new DataReadResult { DataReadFailureReason = DataReadFailureReason.InvalidDirectory };
            }
            catch (UnauthorizedAccessException)
            {
                return new DataReadResult { DataReadFailureReason = DataReadFailureReason.UnauthorizedAccess };
            }
            catch (FileNotFoundException)
            {
                return new DataReadResult { DataReadFailureReason = DataReadFailureReason.FileNotFound };
            }
            catch (NotSupportedException)
            {
                return new DataReadResult { DataReadFailureReason = DataReadFailureReason.NotSupported };
            }
            catch (ArgumentException)
            {
                return new DataReadResult { DataReadFailureReason = DataReadFailureReason.GenericArgumentProblem };
            }
            catch (IOException)
            {
                return new DataReadResult { DataReadFailureReason = DataReadFailureReason.GenericIOProblem };
            }
        }

        /// <summary>
        /// Creates the required directory structure in the user's application data roaming folder, if it does not yet exist.
        /// </summary>
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

        /// <summary>
        /// Creates a directory for a new <see cref="Project"/> in the application storage.
        /// </summary>
        /// <param name="newProject">A <see cref="Project"/> data object that the user wishes to create.</param>
        public void CreateProject(Project newProject)
        {
            string newProjectDirectory = _fileSystem.Path.Combine(ProjectsDirectory, newProject.ProjectID);
            if (_fileSystem.Directory.Exists(newProjectDirectory))
            {
                return;
            }

            _fileSystem.Directory.CreateDirectory(newProjectDirectory);
        }

        /// <summary>
        /// Deletes the directory recursively for a project that the user no longer needs.
        /// </summary>
        /// <param name="existingProject">The project that should be deleted.</param>
        public void DeleteProject(Project existingProject)
        {
            string existingProjectDirectory = _fileSystem.Path.Combine(ProjectsDirectory, existingProject.ProjectID);
            if (_fileSystem.Directory.Exists(existingProjectDirectory))
            {
                _fileSystem.Directory.Delete(existingProjectDirectory, true);
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// Finds all project directories, creates <see cref="Project"/> objects to represent the contents and returns them as a collection that can be observed by the UI.
        /// </summary>
        /// <returns>An <see cref="ObservableCollection{T}"/> of all the projects on the local file system.</returns>
        public ObservableCollection<Project> LoadProjects()
        {
            var projects = new ObservableCollection<Project>();
            string[] projectDirectories = _fileSystem.Directory.GetDirectories(ProjectsDirectory);
            foreach (var projectDirectory in projectDirectories)
            {
                string projectID = projectDirectory.Split('\\').Last();
                var project = new Project(projectID);

                // Each project directory contains child directories for each analytical run the project contains.
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

        /// <summary>
        /// Loads the <see cref="RegressionData"/> for a requested analytical run into an <see cref="AnalyticalRun"/> object.
        /// </summary>
        /// <param name="project">The <see cref="Project"/> that contains the requested <see cref="AnalyticalRun"/>.</param>
        /// <param name="analyticalRunID">The ID string representing the <see cref="AnalyticalRun"/>.</param>
        /// <returns>The requested <see cref="AnalyticalRun"/> object with the <see cref="RegressionData"/> loaded.</returns>
        /// <exception cref="FileNotFoundException">The analytical run with the passed ID could not be found on the file system.</exception>
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

        /// <summary>
        /// Saves an <see cref="AnalyticalRun"/> by creating a new directory in the current <see cref="Project"/> and serialising the <see cref="RegressionData"/> into a json file.
        /// </summary>
        /// <param name="analyticalRun">The <see cref="AnalyticalRun"/> to be saved.</param>
        /// <exception cref="DirectoryNotFoundException">The parent project (which the user should have selected, so should exist) was not found on the file system.</exception>
        public void SaveAnalyticalRun(AnalyticalRun analyticalRun)
        {
            if (string.IsNullOrEmpty(analyticalRun.AnalyticalRunID))
            {
                // User has not chosen an analytical run ID
                return;
            }

            var projects = from p in _fileSystem.Directory.GetDirectories(ProjectsDirectory)
                           select p.Split('\\').Last();
            if (!projects.Contains(analyticalRun.ParentProjectID))
            {
                throw new DirectoryNotFoundException("No project exists for this analytical run");
            }

            string analyticalRunDirectory = _fileSystem.Path.Combine(ProjectsDirectory, analyticalRun.ParentProjectID, analyticalRun.AnalyticalRunID);
            if (!_fileSystem.Directory.Exists(analyticalRunDirectory))
            {
                _fileSystem.Directory.CreateDirectory(analyticalRunDirectory);
            }

            string filePath = _fileSystem.Path.Combine(analyticalRunDirectory, analyticalRun.AnalyticalRunID + ".json");

            string jsonDoc = JsonSerializer.Serialize(analyticalRun.RegressionData);
            _fileSystem.File.WriteAllText(filePath, jsonDoc);
        }

        /// <summary>
        /// Deletes the chosen <see cref="AnalyticalRun"/> from the file system.
        /// </summary>
        /// <param name="project">The <see cref="Project"/> that contains the <see cref="AnalyticalRun"/> to be deleted.</param>
        /// <param name="analyticalRunID">The string ID of the analytical run to be deleted.</param>
        public void DeleteAnalyticalRun(Project project, string analyticalRunID)
        {
            string analyticalRunDirectory = _fileSystem.Path.Combine(ProjectsDirectory, project.ProjectID, analyticalRunID);
            if (_fileSystem.Directory.Exists(analyticalRunDirectory))
            {
                _fileSystem.Directory.Delete(analyticalRunDirectory, true);
            }
            else
            {
                return;
            }
        }

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

        private static string GetProjectsDirectory()
        {
            const string PROJECTSDIRECTORYNAME = "Projects";
            if (ApplicationDirectory == string.Empty)
            {
                return string.Empty;
            }

            return Path.Combine(ApplicationDirectory, PROJECTSDIRECTORYNAME);
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
            else
            {
                return false;
            }
        }
    }
}
