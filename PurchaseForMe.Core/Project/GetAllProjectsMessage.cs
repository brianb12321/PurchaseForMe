using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseForMe.Core.Project
{
    public class GetAllProjectsMessage
    {
    }

    public class ProjectEnumerationMessage
    {
        public IReadOnlyList<ProjectSummary> EnumeratedProjects { get; }

        public ProjectEnumerationMessage(List<ProjectSummary> projects)
        {
            EnumeratedProjects = projects;
        }
    }

    public class ProjectSummary
    {
        public Guid ProjectGuid { get; }
        public string ProjectName { get; }

        public ProjectSummary(Guid projectGuid, string projectName)
        {
            ProjectGuid = projectGuid;
            ProjectName = projectName;
        }
    }
}