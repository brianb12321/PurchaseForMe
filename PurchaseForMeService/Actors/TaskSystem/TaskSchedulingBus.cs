using Akka.Actor;
using Microsoft.Extensions.Logging;
using PurchaseForMe.Core;
using PurchaseForMe.Core.Code.Abstraction;
using PurchaseForMe.Core.Code.Runner;
using PurchaseForMe.Core.TaskSystem;
using PurchaseForMe.Core.TaskSystem.TaskRunner;

namespace PurchaseForMeService.Actors.TaskSystem
{
    public class TaskSchedulingBus : SchedulingBus
    {
        private readonly ILogger<TaskSchedulingBus> _logger;
        public TaskSchedulingBus(ILogger<TaskSchedulingBus> logger, IActorRef pipelineBus, ICodeContextFactory factory)
        {
            _logger = logger;
            for (int i = 0; i < 5; i++)
            {
                IActorRef taskRunner = Context.ActorOf(Props.Create(() => new CodeRunner<TaskRunnerInstance>(i, Self, () => new object[] {pipelineBus, factory}, "taskInstance")), $"taskRunner-{i}");
                RunnerInstances.Add(new RunnerInfo(taskRunner, i));
            }
            Receive<ScheduleTaskImmediatelyMessage>(message =>
            {
                var runner = SendMessageToOpenRunner(new RunnerStartMessage(message.TaskNode.BlocklyWorkspace.InnerXml, message), Sender);
                if (runner == null)
                {
                    _logger.LogInformation("No runners available.");
                    Sender.Tell(new NoRunnerAvailableMessage(message, message.TaskNode.NodeGuid));
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
                runner.CallingActor.Tell((TaskCompleted)message.Result);
                runner.CallingActor = null;
                //If any messages were stashed, un-stash now.
                Stash.Unstash();
            });
        }
    }
}