using System;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PurchaseForMe.Core;
using PurchaseForMe.Core.TaskMonitoring;

namespace PurchaseForMe.Hubs
{
    public class CodeMonitorHub : Hub
    {
        private readonly ILogger<CodeMonitorHub> _logger;
        private readonly IActorRef _signalRActor;
        public CodeMonitorHub(ILogger<CodeMonitorHub> logger, CodeMonitorSignalRFactory signalR)
        {
            _logger = logger;
            _signalRActor = signalR();
        }

        public Task RegisterToCodeMonitor(string taskGuid)
        {
            _signalRActor.Tell(new ClientConnectedMessage(Guid.Parse(taskGuid), Context.ConnectionId));
            return Task.CompletedTask;
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            _signalRActor.Tell(new ClientDisconnectedMessage(Context.ConnectionId));
            return Task.CompletedTask;
        }

        public Task RunCodeBlockly(string requestJson)
        {
            _logger.LogInformation("Starting block code.");
            CodeMonitorSignalR.RunCodeSignalR request = JsonConvert.DeserializeObject<CodeMonitorSignalR.RunCodeSignalR>(requestJson);
            request.ClientId = Context.ConnectionId;
            _signalRActor.Tell(request);
            return Task.CompletedTask;
        }
    }
}