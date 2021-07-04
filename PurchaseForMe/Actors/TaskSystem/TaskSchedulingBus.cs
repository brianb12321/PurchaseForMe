using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Microsoft.Extensions.Logging;
using PurchaseForMe.Core.Code.Runner;
using PurchaseForMe.Core.TaskSystem;
using PurchaseForMe.Core.TaskSystem.TaskRunner;

namespace PurchaseForMe.Actors.TaskSystem
{
    public delegate IActorRef TaskSchedulingBusFactory();
    public class TaskSchedulingBus : SchedulingBus
    {
        private readonly ILogger<TaskSchedulingBus> _logger;
        public TaskSchedulingBus(ILogger<TaskSchedulingBus> logger)
        {
            _logger = logger;
            for (int i = 1; i <= 5; i++)
            {
                IActorRef taskRunner = Context.ActorOf(Props.Create(() => new CodeRunner(i, Self)), $"taskRunner-{i}");
                RunnerInstances.Add(new RunnerInfo(taskRunner, i));
            }
            Receive<ScheduleTaskImmediatelyMessage>(message =>
            {
                var runner = SendMessageToOpenRunner(new RunnerStartMessage(message.WorkspaceXml, message), Sender);
                if (runner == null)
                {
                    _logger.LogInformation("No runners available.");
                    Sender.Tell(new NoRunnerAvailableMessage(message));
                }
                else
                {
                    _logger.LogInformation($"Runner {runner.RunnerId} selected for work.");
                }
            });
            Receive<RunnerStartedMessage>(message =>
            {
                //TODO: Check for invalid Id.
                RunnerInstances[message.RunnerId].IsRunning = true;
            });
            Receive<RunnerFinishedMessage>(message =>
            {
                var runner = RunnerInstances[message.RunnerId];
                runner.IsRunning = false;
                runner.CallingActor = null;
                //If any messages were stashed, un-stash now.
                Stash.Unstash();
            });
        }
    }
}