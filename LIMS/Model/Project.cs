using System.Collections.Generic;

namespace LIMS.Model
{
    /// <summary>
    /// Record representing a uniquely named project, that can contain multiple analytical runs.
    /// </summary>
    public record Project
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Project"/> class.
        /// </summary>
        /// <param name="projectID">The unique ID of the project to be created.</param>
        public Project(string projectID)
        {
            ProjectID = projectID;
        }

        /// <summary>
        /// Gets the ID of the project.
        /// </summary>
        public string ProjectID { get; }

        /// <summary>
        /// Gets a hashset of the unique analytical run IDs in this project.
        /// </summary>
        public HashSet<string> AnalyticalRunIDs { get; } = new();
    }
}
