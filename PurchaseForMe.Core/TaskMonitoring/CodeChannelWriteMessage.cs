using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseForMe.Core.TaskMonitoring
{
    public class CodeChannelWriteMessage
    {
        public Guid CodeGuid { get; }
        public string Message { get; }

        public CodeChannelWriteMessage(Guid codeGuid, string message)
        {
            CodeGuid = codeGuid;
            Message = message;
        }
    }
}