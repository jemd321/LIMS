using System.Collections.ObjectModel;
using System.IO;
using LIMS.Model;

namespace LIMS.Data
{
    /// <summary>
    /// Provides CRUD data storage service to the application.
    /// </summary>
    public interface IDataService
    {
        /// <summary>
        /// Initialises the storage to be used by the application.
        /// </summary>
        void SetupApplicationStorage();

        /// <summary>
        /// Creates a new <see cref=Project"/> in the application storage.
        /// </summary>
        /// <param name="newProject">A <see cref="Project"/> data object that the user wishes to create.</param>
        void CreateProject(Project newProject);

        /// <summary>
        /// Deletes a <see cref="Project"/> in the application storage that the user no longer needs.
        /// </summary>
        /// <param name="existingProject">The project that should be deleted.</param>
        void DeleteProject(Project existingProject);

        /// <summary>
        /// Loads all <see cref="Project"/> objects and returns them as a collection that can be observed by the UI.
        /// </summary>
        /// <returns>An <see cref="ObservableCollection{T}"/> of all the projects on the local file system.</returns>
        ObservableCollection<Project> LoadProjects();

        /// <summary>
        /// Loads the <see cref="RegressionData"/> for a requested analytical run into an <see cref="AnalyticalRun"/> object.
        /// </summary>
        /// <param name="project">The <see cref="Project"/> that contains the requested <see cref="AnalyticalRun"/>.</param>
        /// <param name="analyticalRunID">The ID string representing the <see cref="AnalyticalRun"/>.</param>
        /// <returns>The requested <see cref="AnalyticalRun"/> object with the <see cref="RegressionData"/> loaded.</returns>
        AnalyticalRun LoadAnalyticalRun(Project project, string analyticalRunID);

        /// <summary>
        /// Saves an <see cref="AnalyticalRun"/> to the application storage, under the currently open <see cref="Project"/>.
        /// </summary>
        /// <param name="analyticalRun">The <see cref="AnalyticalRun"/> to be saved.</param>
        void SaveAnalyticalRun(AnalyticalRun analyticalRun);

        /// <summary>
        /// Deletes the chosen <see cref="AnalyticalRun"/> from the application storage.
        /// </summary>
        /// <param name="project">The <see cref="Project"/> that contains the <see cref="AnalyticalRun"/> to be deleted.</param>
        /// <param name="analyticalRunID">The string ID of the analytical run to be deleted.</param>
        void DeleteAnalyticalRun(Project project, string analyticalRunID);

        FileInfo ValidateFilePath(string filePath);

        string GetRawData(FileInfo fileInfo);
    }
}
