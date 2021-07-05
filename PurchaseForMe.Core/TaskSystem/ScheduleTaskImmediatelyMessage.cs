using System;
using PurchaseForMe.Core.Project;

namespace PurchaseForMe.Core.TaskSystem
{
    public class ScheduleTaskImmediatelyMessage : TaskStartMessage
    {
        public string WorkspaceXml { get; }

        public ScheduleTaskImmediatelyMessage(string workspaceXml, ProjectInstance project)
        {
            WorkspaceXml = workspaceXml;
            Project = project;
            SessionId = Guid.NewGuid();
        }
    }
}