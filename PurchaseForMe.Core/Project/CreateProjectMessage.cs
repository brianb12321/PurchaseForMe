using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseForMe.Core.Project
{
    public class CreateProjectMessage
    {
        public string ProjectName { get; }
        public IReadOnlyList<ProjectNode> ProjectItems { get; }
        public string Description { get; }

        public CreateProjectMessage(string projectName, string description = "", List<ProjectNode> projectItems = null)
        {
            if (projectItems == null)
            {
                ProjectItems = new List<ProjectNode>();
            }
            else
            {
                ProjectItems = projectItems;
            }
            ProjectName = projectName;
            Description = description;
        }
    }

    public class CreateProjectResultMessage
    {
        public bool IsSuccessful { get; }
        public string Message { get; }
        public ProjectSummary Summary { get; }

        public CreateProjectResultMessage(bool isSuccessful, string message, ProjectSummary summary = null)
        {
            IsSuccessful = isSuccessful;
            Message = message;
            Summary = summary;
        }
    }
}