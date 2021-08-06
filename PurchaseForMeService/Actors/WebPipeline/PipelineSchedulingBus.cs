using System;
using Akka.Actor;
using Microsoft.Extensions.Logging;
using PurchaseForMe.Core.Code.Abstraction;
using PurchaseForMe.Core.Code.Runner;
using PurchaseForMe.Core.TaskSystem.TaskRunner;
using PurchaseForMe.Core.WebPipeline;

namespace PurchaseForMeService.Actors.WebPipeline
{
    public class PipelineSchedulingBus : SchedulingBus
    {
        private readonly ILogger<PipelineSchedulingBus> _logger;

        public PipelineSchedulingBus(ILogger<PipelineSchedulingBus> logger, ICodeContextFactory factory)
        {
            _logger = logger;
            for (int i = 0; i < 5; i++)
            {
                IActorRef taskRunner = Context.ActorOf(Props.Create(() => new CodeRunner<WebPipelineInstanceActor>(i, Self,
                    () => new Object[] {factory}, "pipelineInstance")), $"webPipelineRunner-{i}");
                RunnerInstances.Add(new RunnerInfo(taskRunner, i));
            }

            Receive<PipelineRunRequest>(message =>
            {
                var runner = SendMessageToOpenRunner(new RunnerStartMessage(message.PipelineNode.BlocklyWorkspace.InnerXml, message), Sender);
                if (runner == null)
                {
                    _logger.LogInformation("No runners available.");
                    Sender.Tell(new NoRunnerAvailableMessage(message, message.PipelineNode.NodeGuid));
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