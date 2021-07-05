using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Akka.Actor;
using IronBlock;
using IronBlock.Blocks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PurchaseForMe.Actors.WebPipeline;
using PurchaseForMe.Blocks;
using PurchaseForMe.Core.WebPipeline;
using Workspace = IronBlock.Workspace;

namespace PurchaseForMe.Hubs
{
    public class PipelineRunnerHub : Hub
    {
        private readonly ILogger<PipelineRunnerHub> _logger;
        private readonly IActorRef _signalRActor;
        public PipelineRunnerHub(ILogger<PipelineRunnerHub> logger, WebPipelineSignalRActorFactory signalR)
        {
            _logger = logger;
            _signalRActor = signalR();
        }
        public Task RunPipelineBlockly(string requestJson)
        {
            _logger.LogInformation("Starting block code.");
            PipelineRunRequest request = JsonConvert.DeserializeObject<PipelineRunRequest>(requestJson);
            WebPipelineSignalRRequestMessage signalRRequest =
                new WebPipelineSignalRRequestMessage(Context.ConnectionId, request);
            
            _signalRActor.Tell(signalRRequest);
            return Task.CompletedTask;
        }
    }
}