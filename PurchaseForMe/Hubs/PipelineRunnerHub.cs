using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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
    class SignalRTextWriter : TextWriter
    {
        private readonly IClientProxy _client;

        public SignalRTextWriter(IClientProxy client)
        {
            _client = client;
        }
        public override Encoding Encoding => Encoding.UTF8;
        public override void WriteLine(string? value)
        {
            Write(value + "\n");
        }

        public override void Write(string? value)
        {
            _client.SendAsync("Console", value).GetAwaiter().GetResult();
        }
    }

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