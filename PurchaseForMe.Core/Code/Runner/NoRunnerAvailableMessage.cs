using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseForMe.Core.Code.Runner
{
    public class NoRunnerAvailableMessage
    {
        public object RequestMessage { get; }

        public NoRunnerAvailableMessage(object requestMessage, Guid codeGuid)
        {
            RequestMessage = requestMessage;
        }
    }
}