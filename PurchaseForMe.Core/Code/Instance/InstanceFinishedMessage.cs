namespace PurchaseForMe.Core.Code.Instance
{
    public class InstanceFinishedMessage
    {
        public object Result { get; }
        public bool IsSuccessful { get; }

        public InstanceFinishedMessage(bool isSuccessful, object result)
        {
            IsSuccessful = isSuccessful;
            Result = result;
        }
    }
}