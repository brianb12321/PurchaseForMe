using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PurchaseForMe.Core.Project;

namespace PurchaseForMe.Core.TaskSystem
{
    public abstract class TaskStartMessage
    {
        public Guid SessionId { get; protected set; }
        public ProjectInstance Project { get; protected set; }
    }
}