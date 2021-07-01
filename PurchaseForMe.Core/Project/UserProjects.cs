using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PurchaseForMe.Core.User;

namespace PurchaseForMe.Core.Project
{
    public class UserProjects
    {
        public string UserId { get; }
        public ConcurrentBag<ProjectInstance> OpenedProjects { get; }

        public UserProjects(string userId)
        {
            UserId = userId;
            OpenedProjects = new ConcurrentBag<ProjectInstance>();
        }
    }
}