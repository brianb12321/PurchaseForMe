using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.Extensions.Logging;
using PurchaseForMe.Core.Code.Runner;
using PurchaseForMe.Core.TaskSystem.TaskRunner;
using PurchaseForMe.Core.WebPipeline;

namespace PurchaseForMe.Actors.WebPipeline
{
    public delegate IActorRef PipelineSchedulingBusFactory();
    public class PipelineSchedulingBus : SchedulingBus
    {
        private readonly ILogger<PipelineSchedulingBus> _logger;

        public PipelineSchedulingBus(ILogger<PipelineSchedulingBus> logger)
        {
            _logger = logger;
            for (int i = 0; i < 5; i++)
            {
                IActorRef taskRunner = Context.ActorOf(Props.Create(() => new CodeRunner(i, Self)), $"webPipelineRunner-{i}");
                RunnerInstances.Add(new RunnerInfo(taskRunner, i));
            }

            Receive<PipelineRunRequest>(message =>
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
                RunnerInstances[message.RunnerId].IsRunning = true;
            });
            Receive<RunnerFinishedMessage>(message =>
            {
                var runner = RunnerInstances[message.RunnerId];
                runner.IsRunning = false;
                runner.CallingActor.Tell((PipelineInstanceResult)message.Result);
                runner.CallingActor = null;
                //If any messages were stashed, un-stash now.
                Stash.Unstash();
            });
        }
    }
}