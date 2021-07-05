namespace PurchaseForMe.Core.Code.Instance
{
    public class InstanceStartMessage
    {
        public object AdditionalData { get; }
        public string WorkspaceXml { get; }

        public InstanceStartMessage(string workspaceXml, object additionalData = null)
        {
            WorkspaceXml = workspaceXml;
            AdditionalData = additionalData;
        }
    }
}