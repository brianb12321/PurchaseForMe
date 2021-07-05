using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using PurchaseForMe.Actors.Project;
using PurchaseForMe.Actors.WebPipeline;
using PurchaseForMe.Core.Code.Runner;
using PurchaseForMe.Core.Project;
using PurchaseForMe.Core.Project.Nodes.Blockly;
using PurchaseForMe.Core.TaskSystem;
using PurchaseForMe.Hubs;

namespace PurchaseForMe.Actors.TaskSystem
{
    public delegate IActorRef TaskSchedulingSignalRFactory();
    public class TaskSchedulingSignalR : ReceiveActor
    {
        public class ScheduleTaskImmediatelySignalRMessage
        {
            public string ClientId { get; set; }
            public string ProjectGuid { get; set; }
            public string NodeGuid { get; set; }
            public string UserId { get; set; }
        }

        private readonly IHubContext<TaskRunnerHub> _context;
        private readonly IActorRef _taskSchedulingBus;
        private readonly IActorRef _projectManager;
        private readonly ConcurrentDictionary<Guid, string> _clientsToSessions;
        public TaskSchedulingSignalR(IHubContext<TaskRunnerHub> context, TaskSchedulingBusFactory taskBus, ProjectManagerFactory projectManager)
        {
            _clientsToSessions = new ConcurrentDictionary<Guid, string>();
            _context = context;
            _taskSchedulingBus = taskBus();
            _projectManager = projectManager();
            ReceiveAsync<ScheduleTaskImmediatelySignalRMessage>(async request =>
            {
                Guid projectGuid = Guid.Parse(request.ProjectGuid);
                Guid nodeGuid = Guid.Parse(request.NodeGuid);
                var response = await _projectManager.Ask<GetProjectResponseMessage>(new GetProjectMessage(projectGuid, request.UserId));
                BlocklyTaskNode taskNode = (BlocklyTaskNode)response.Project.ProjectItems.First(i =>
                    i.NodeGuid == nodeGuid && i.NodeType == NodeType.BlocklyTask);

                var message = new ScheduleTaskImmediatelyMessage(taskNode.BlocklyWorkspace.InnerXml, response.Project);
                _clientsToSessions.TryAdd(message.SessionId, request.ClientId);
                _taskSchedulingBus.Tell(message, Self);
            });
            ReceiveAsync<NoRunnerAvailableMessage>(async message =>
            {
                ScheduleTaskImmediatelyMessage originalMessage = (ScheduleTaskImmediatelyMessage)message.RequestMessage;
                _clientsToSessions.Remove(originalMessage.SessionId, out string clientId);
                await _context.Clients.Client(clientId).SendAsync("Console", $"No runners are available to process your code. Please try again later.{Environment.NewLine}");
            });
            ReceiveAsync<TaskCompleted>(async result =>
            {
                _clientsToSessions.Remove(result.SessionId, out string clientId);
                await _context.Clients.Client(clientId).SendAsync("Result", JObject.FromObject(result).ToString());
            });
        }
    }
}
