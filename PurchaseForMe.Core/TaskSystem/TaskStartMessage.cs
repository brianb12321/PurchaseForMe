using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PurchaseForMe.Core.Project;
using PurchaseForMe.Core.Project.Nodes.Blockly;

namespace PurchaseForMe.Core.TaskSystem
{
    public abstract class TaskStartMessage
    {
        public ProjectInstance Project { get; protected set; }
        public BlocklyTaskNode TaskNode { get; protected set; }
    }
}