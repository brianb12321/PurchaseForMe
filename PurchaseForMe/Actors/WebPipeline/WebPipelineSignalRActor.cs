using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using PurchaseForMe.Core.Code.Runner;
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
        private readonly IActorRef _pipelineSchedulingBus;
        private readonly ConcurrentDictionary<Guid, string> _clientsToSessions;
        public WebPipelineSignalRActor(IHubContext<PipelineRunnerHub> context, PipelineSchedulingBusFactory pipeline)
        {
            _clientsToSessions = new ConcurrentDictionary<Guid, string>();
            _context = context;
            _pipelineSchedulingBus = pipeline();
            Receive<WebPipelineSignalRRequestMessage>(request =>
            {
                _clientsToSessions.TryAdd(request.Request.SessionId, request.ClientId);
                _pipelineSchedulingBus.Tell(request.Request, Self);
            });
            ReceiveAsync<NoRunnerAvailableMessage>(async message =>
            {
                PipelineRunRequest originalMessage = (PipelineRunRequest) message.RequestMessage;
                _clientsToSessions.Remove(originalMessage.SessionId, out string clientId);
                await _context.Clients.Client(clientId).SendAsync("Console", $"No runners are available to process your code. Please try again later.{Environment.NewLine}");
            });
            ReceiveAsync<PipelineInstanceResult>(async result =>
            {
                _clientsToSessions.Remove(result.SessionId, out string clientId);
                await _context.Clients.Client(clientId).SendAsync("Result", JObject.FromObject(result).ToString());
            });
        }
    }
}