namespace PurchaseForMe.Core.Code.Runner
{
    public class RunnerStartMessage
    {
        public string WorkspaceXml { get; }
        public object AdditionalData { get; }
        public RunnerStartMessage(string workspaceXml, object additionalData = null)
        {
            WorkspaceXml = workspaceXml;
            AdditionalData = additionalData;
        }
    }
}