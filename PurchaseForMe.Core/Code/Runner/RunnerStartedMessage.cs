namespace PurchaseForMe.Core.TaskSystem.TaskRunner
{
    public class RunnerStartedMessage : RunnerMessage
    {
        public RunnerStartedMessage(int runnerId) : base(runnerId)
        {
        }
    }
}