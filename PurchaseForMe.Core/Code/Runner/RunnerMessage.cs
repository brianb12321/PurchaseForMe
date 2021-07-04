namespace PurchaseForMe.Core.TaskSystem.TaskRunner
{
    public abstract class RunnerMessage
    {
        public int RunnerId { get; }

        protected RunnerMessage(int runnerId)
        {
            RunnerId = runnerId;
        }
    }
}