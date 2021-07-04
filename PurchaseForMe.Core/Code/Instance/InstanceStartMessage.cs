namespace PurchaseForMe.Core.Code.Instance
{
    public class InstanceStartMessage
    {
        public object AdditionalData { get; }

        public InstanceStartMessage(object additionalData = null)
        {
            AdditionalData = additionalData;
        }
    }
}