using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseForMe.Core.TaskSystem
{
    public class TaskCompleted
    {
        public Guid SessionId { get; set; }
        public bool IsSuccessful { get; set; }
        public string ResultMessage { get; set; }

        public TaskCompleted(Guid sessionId, bool isSuccessful, string resultMessage)
        {
            SessionId = sessionId;
            IsSuccessful = isSuccessful;
            ResultMessage = resultMessage;
        }
    }
}