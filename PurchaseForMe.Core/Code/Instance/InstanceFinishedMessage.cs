namespace PurchaseForMe.Core.Code.Instance
{
    public class InstanceFinishedMessage
    {
        public object Result { get; }

        public InstanceFinishedMessage(object result)
        {
            Result = result;
        }
    }
}