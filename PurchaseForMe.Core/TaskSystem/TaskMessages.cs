using PurchaseForMe.Core.Project;
using PurchaseForMe.Core.Project.Nodes.Blockly;

namespace PurchaseForMe.Core.TaskSystem
{
    public abstract record TaskStartMessage(ProjectInstance Project, BlocklyTaskNode TaskNode);

    public record ScheduleTaskImmediatelyMessage(ProjectInstance Project, BlocklyTaskNode TaskNode) : TaskStartMessage(
        Project, TaskNode);
}