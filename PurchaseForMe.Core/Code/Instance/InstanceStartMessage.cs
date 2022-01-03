namespace PurchaseForMe.Core.Code.Instance
{
    public class InstanceStartMessage
    {
        public object AdditionalData { get; }
        public string Code { get; }

        public InstanceStartMessage(string code, object additionalData = null)
        {
            Code = code;
            AdditionalData = additionalData;
        }
    }
}