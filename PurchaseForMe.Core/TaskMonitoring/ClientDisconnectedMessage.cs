using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseForMe.Core.TaskMonitoring
{
    public class ClientDisconnectedMessage
    {
        public string ClientId { get; }

        public ClientDisconnectedMessage(string clientId)
        {
            ClientId = clientId;
        }
    }
}