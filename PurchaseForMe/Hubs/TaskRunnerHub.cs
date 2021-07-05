using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PurchaseForMe.Actors.TaskSystem;
using PurchaseForMe.Actors.WebPipeline;
using PurchaseForMe.Core.TaskSystem;
using PurchaseForMe.Core.WebPipeline;

namespace PurchaseForMe.Hubs
{
    public class TaskRunnerHub : Hub
    {
        private readonly ILogger<TaskRunnerHub> _logger;
        private readonly IActorRef _signalRActor;
        public TaskRunnerHub(ILogger<TaskRunnerHub> logger, TaskSchedulingSignalRFactory signalR)
        {
            _logger = logger;
            _signalRActor = signalR();
        }
        public Task RunTaskBlockly(string requestJson)
        {
            _logger.LogInformation("Starting block code.");
            TaskSchedulingSignalR.ScheduleTaskImmediatelySignalRMessage request = JsonConvert.DeserializeObject<TaskSchedulingSignalR.ScheduleTaskImmediatelySignalRMessage>(requestJson);
            request.ClientId = Context.ConnectionId;
            _signalRActor.Tell(request);
            return Task.CompletedTask;
        }
    }
}