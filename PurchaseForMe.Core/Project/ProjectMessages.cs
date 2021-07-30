using System;
using System.Collections.Generic;

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

    public record CreateProjectResultMessage(bool IsSuccessful, string Message, ProjectSummary Summary = null);

    public record GetAllProjectsMessage();

    public record ProjectEnumerationMessage(IReadOnlyList<ProjectSummary> EnumeratedProjects);

    public record ProjectSummary(Guid ProjectGuid, string ProjectName);

    public record LoadProjectMessage(Guid ProjectGuid);

    public record SaveProjectMessage(Guid ProjectGuid, string UserId = "");
}