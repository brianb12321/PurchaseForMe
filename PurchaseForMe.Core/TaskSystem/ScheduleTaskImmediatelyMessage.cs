namespace PurchaseForMe.Core.TaskSystem
{
    public class ScheduleTaskImmediatelyMessage
    {
        public string WorkspaceXml { get; }

        public ScheduleTaskImmediatelyMessage(string workspaceXml)
        {
            WorkspaceXml = workspaceXml;
        }
    }
}