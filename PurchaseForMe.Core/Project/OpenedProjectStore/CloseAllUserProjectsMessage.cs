using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseForMe.Core.Project
{
    public class CloseAllUserProjectsMessage
    {
        public string UserId { get; }

        public CloseAllUserProjectsMessage(string userId)
        {
            UserId = userId;
        }
    }
}