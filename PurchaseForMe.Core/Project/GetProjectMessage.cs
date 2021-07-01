using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseForMe.Core.Project
{
    public class GetProjectMessage
    {
        public Guid ProjectGuid { get; }
        public string UserId { get; }

        public GetProjectMessage(Guid projectGuid, string userId = "")
        {
            ProjectGuid = projectGuid;
            UserId = userId;
        }
    }

    public class GetProjectResponseMessage
    {
        public ProjectInstance Project { get; }

        public GetProjectResponseMessage(ProjectInstance project)
        {
            Project = project;
        }
    }
}