using PurchaseForMe.Core.TaskSystem.TaskRunner;

namespace PurchaseForMe.Core.Code.Runner
{
    public class RunnerFinishedMessage : RunnerMessage
    {
        public object Result { get; }
        public RunnerFinishedMessage(int runnerId, object result) : base(runnerId)
        {
            Result = result;
        }
    }
}