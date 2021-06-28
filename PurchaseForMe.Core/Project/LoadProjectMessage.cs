using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseForMe.Core.Project
{
    public class LoadProjectMessage
    {
        public Guid ProjectGuid { get; }

        public LoadProjectMessage(Guid projectGuid)
        {
            ProjectGuid = projectGuid;
        }
    }
}