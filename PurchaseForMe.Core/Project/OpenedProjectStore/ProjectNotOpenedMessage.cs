using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseForMe.Core.Project.OpenedProjectStore
{
    public class ProjectNotOpenedMessage
    {
        public Guid ProjectGuid { get; }
        public string UserId { get; }

        public ProjectNotOpenedMessage(Guid projectGuid, string userId)
        {
            ProjectGuid = projectGuid;
            UserId = userId;
        }
    }
}