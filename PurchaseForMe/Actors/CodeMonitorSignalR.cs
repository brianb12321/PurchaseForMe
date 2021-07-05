using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Util;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using PurchaseForMe.Actors.Project;
using PurchaseForMe.Actors.TaskSystem;
using PurchaseForMe.Actors.WebPipeline;
using PurchaseForMe.Core.Code.Runner;
using PurchaseForMe.Core.Project;
using PurchaseForMe.Core.Project.Nodes.Blockly;
using PurchaseForMe.Core.TaskMonitoring;
using PurchaseForMe.Core.TaskSystem;
using PurchaseForMe.Core.WebPipeline;
using PurchaseForMe.Hubs;

namespace PurchaseForMe.Actors
{
    public delegate IActorRef CodeMonitorSignalRFactory();
    public class CodeMonitorSignalR : ReceiveActor
    {
        public class RunCodeSignalR
        {
            public string ClientId { get; set; }
            public string ProjectGuid { get; set; }
            public string NodeGuid { get; set; }
            public string UserId { get; set; }
        }

        private readonly IHubContext<CodeMonitorHub> _context;
        private readonly IActorRef _taskSchedulingBus;
        private readonly IActorRef _pipelineSchedulingBus;
        private readonly IActorRef _projectManager;
        private readonly ConcurrentDictionary<Guid, List<string>> _connectedClients;
        public CodeMonitorSignalR(IHubContext<CodeMonitorHub> context, TaskSchedulingBusFactory taskBus, PipelineSchedulingBusFactory pipelineBus, ProjectManagerFactory projectManager)
        {
            _connectedClients = new ConcurrentDictionary<Guid, List<string>>();
            _context = context;
            _taskSchedulingBus = taskBus();
            _pipelineSchedulingBus = pipelineBus();
            _projectManager = projectManager();

            //Subscribe to code output messages.
            Context.System.EventStream.Subscribe(Self, typeof(CodeChannelWriteMessage));

            Receive<CodeChannelWriteMessage>(message =>
            {
                if (_connectedClients.ContainsKey(message.CodeGuid))
                {
                    _connectedClients[message.CodeGuid].ForEach(clientId =>
                    {
                        _context.Clients.Client(clientId).SendAsync("Console", message.Message);
                    });
                }
            });
            Receive<ClientConnectedMessage>(message =>
            {
                var keyExisted = _connectedClients.GetOrAdd(message.TaskGuid, new List<string>());
                //Task already has subscribers.
                if (keyExisted != null)
                {
                    keyExisted.Add(message.ClientId);
                }
            });
            Receive<ClientDisconnectedMessage>(message =>
            {
                foreach (List<string> clients in _connectedClients.Values)
                {
                    if (clients.Contains(message.ClientId))
                    {
                        clients.Remove(message.ClientId);
                    }
                }
            });
            ReceiveAsync<RunCodeSignalR>(async request =>
            {
                Guid projectGuid = Guid.Parse(request.ProjectGuid);
                Guid nodeGuid = Guid.Parse(request.NodeGuid);
                var response = await _projectManager.Ask<GetProjectResponseMessage>(new GetProjectMessage(projectGuid, request.UserId));
                BlocklyNode codeNode = (BlocklyNode)response.Project.ProjectItems.First(i =>
                    i.NodeGuid == nodeGuid);
                if (codeNode.NodeType == NodeType.BlocklyTask)
                {
                    var message = new ScheduleTaskImmediatelyMessage((BlocklyTaskNode)codeNode, response.Project);
                    _taskSchedulingBus.Tell(message, Self);
                }
                else if (codeNode.NodeType == NodeType.BlocklyPipeline)
                {
                    var message = new PipelineRunRequest();
                    message.PipelineNode = (BlocklyPipelineNode)codeNode;
                    _pipelineSchedulingBus.Tell(message, Self);
                }
            });
            ReceiveAsync<NoRunnerAvailableMessage>(async message =>
            {
                ScheduleTaskImmediatelyMessage originalMessage = (ScheduleTaskImmediatelyMessage)message.RequestMessage;
                if (_connectedClients.ContainsKey(originalMessage.TaskNode.NodeGuid))
                {
                    _connectedClients[originalMessage.TaskNode.NodeGuid].ForEach(async clientId =>
                    {
                        await _context.Clients.Client(clientId).SendAsync("Console", $"No runners are available to process your code. Please try again later.{Environment.NewLine}");
                    });
                }
            });
            ReceiveAsync<PipelineInstanceResult>(async result =>
            {
                if (_connectedClients.ContainsKey(result.CodeGuid))
                {
                    _connectedClients[result.CodeGuid].ForEach(async clientId =>
                    {
                        await _context.Clients.Client(clientId).SendAsync("Result", JObject.FromObject(result).ToString());
                    });
                }
            });
            ReceiveAsync<TaskCompleted>(async result =>
            {
                if (_connectedClients.ContainsKey(result.CodeGuid))
                {
                    _connectedClients[result.CodeGuid].ForEach(async clientId =>
                    {
                        await _context.Clients.Client(clientId).SendAsync("Result", JObject.FromObject(result).ToString());
                    });
                }
            });
        }
    }
}
