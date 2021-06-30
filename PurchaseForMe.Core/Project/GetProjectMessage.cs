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

        public GetProjectMessage(Guid projectGuid)
        {
            ProjectGuid = projectGuid;
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