using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseForMe.Core.Project.OpenedProjectStore
{
    public class GetOpenedProjectMessage
    {
        public Guid ProjectGuid { get; }
        public string UserId { get; }

        public GetOpenedProjectMessage(string userId, Guid projectGuid)
        {
            UserId = userId;
            ProjectGuid = projectGuid;
        }
    }
}