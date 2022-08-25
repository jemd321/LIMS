using System.Collections.Generic;

namespace LIMS.Model
{
    public record Project
    {
        public Project(string projectID)
        {
            ProjectID = projectID;
        }

        public string ProjectID { get; }

        public HashSet<string> AnalyticalRunIDs { get; } = new();
    }
}
