using System.Collections.Generic;
using PurchaseForMe.Core.Code.Runner;
using PurchaseForMe.Core.Project;
using PurchaseForMe.Core.Project.Nodes.Blockly;

namespace PurchaseForMe.Core.TaskSystem
{
    public abstract record TaskStartMessage(ProjectInstance Project, BlocklyTaskNode TaskNode);

    public record GetTaskRunnerInfoForAll();

    public record TaskRunnerInfoEnumeration(IReadOnlyList<RunnerInfo> Runners);

    public record ScheduleTaskImmediatelyMessage(ProjectInstance Project, BlocklyTaskNode TaskNode) : TaskStartMessage(
        Project, TaskNode);
}