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
        public PurchaseForMeUser User { get; }
        public ConcurrentBag<ProjectInstance> OpenedProjects { get; }

        public UserProjects(PurchaseForMeUser user)
        {
            User = user;
            OpenedProjects = new ConcurrentBag<ProjectInstance>();
        }
    }
}