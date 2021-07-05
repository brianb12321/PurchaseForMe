using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseForMe.Core.TaskMonitoring
{
    public class ClientConnectedMessage
    {
        public Guid TaskGuid { get; }
        public string ClientId { get; }

        public ClientConnectedMessage(Guid taskGuid, string clientId)
        {
            TaskGuid = taskGuid;
            ClientId = clientId;
        }
    }
}