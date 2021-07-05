using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PurchaseForMe.Core.Code;

namespace PurchaseForMe.Core.TaskSystem
{
    public class TaskCompleted : CodeResult
    {
        public TaskCompleted(Guid taskGuid, bool isSuccessful, string resultMessage)
        {
            CodeGuid = taskGuid;
            IsSuccessful = isSuccessful;
            ResultMessage = resultMessage;
        }
    }
}