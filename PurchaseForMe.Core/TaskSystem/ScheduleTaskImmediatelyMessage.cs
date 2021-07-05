using System;
using PurchaseForMe.Core.Project;
using PurchaseForMe.Core.Project.Nodes.Blockly;

namespace PurchaseForMe.Core.TaskSystem
{
    public class ScheduleTaskImmediatelyMessage : TaskStartMessage
    {
        public ScheduleTaskImmediatelyMessage(BlocklyTaskNode taskNode, ProjectInstance project)
        {
            TaskNode = taskNode;
            Project = project;
        }
    }
}