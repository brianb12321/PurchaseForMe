using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using PurchaseForMe.Core.WebPipeline;
using PurchaseForMe.Hubs;

namespace PurchaseForMe.Actors.WebPipeline
{
    public delegate IActorRef WebPipelineSignalRActorFactory();
    public class WebPipelineSignalRRequestMessage
    {
        public string ClientId { get; }
        public PipelineRunRequest Request { get; }

        public WebPipelineSignalRRequestMessage(string clientId, PipelineRunRequest request)
        {
            ClientId = clientId;
            Request = request;
        }
    }
    public class WebPipelineSignalRActor : ReceiveActor
    {
        private readonly IHubContext<PipelineRunnerHub> _context;
        private string _currentConnectedClientId;
        private readonly IActorRef _pipelineActor;
        public WebPipelineSignalRActor(IHubContext<PipelineRunnerHub> context, WebPipelineActorFactory pipeline)
        {
            _context = context;
            _pipelineActor = pipeline();
            Become(PipelineReady);
        }

        protected void PipelineRunning()
        {
            ReceiveAsync<PipelineInstanceResult>(async result =>
            {
                await _context.Clients.Client(_currentConnectedClientId).SendAsync("Result", JObject.FromObject(result).ToString());
                Become(PipelineReady);
            });
        }

        protected void PipelineReady()
        {
            _currentConnectedClientId = string.Empty;
            Receive<WebPipelineSignalRRequestMessage>(request =>
            {
                _currentConnectedClientId = request.ClientId;
                _pipelineActor.Tell(request.Request, Self);
                Become(PipelineRunning);
            });
        }
    }
}