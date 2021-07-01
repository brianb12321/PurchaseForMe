using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseForMe.Core.Project.OpenedProjectStore
{
    public class OpenProjectMessage
    {
        public ProjectInstance Project { get; }
        public string UserId { get; }

        public OpenProjectMessage(string userId, ProjectInstance project)
        {
            UserId = userId;
            Project = project;
        }
    }
}