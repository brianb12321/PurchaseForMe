namespace PurchaseForMe.Core.Project.OpenedProjectStore
{
    public class CloseAllUserProjectsMessage
    {
        public string UserId { get; }

        public CloseAllUserProjectsMessage(string userId)
        {
            UserId = userId;
        }
    }
}