using System;
using Akka.Actor;
using PurchaseForMe.Actors.WebPipeline;
using PurchaseForMe.Core.Code.Instance;
using PurchaseForMe.Core.Code.Runner;
using PurchaseForMe.Core.TaskSystem.TaskRunner;

namespace PurchaseForMe.Actors
{
    public class CodeRunner : ReceiveActor
    {
        public int RunnerId { get; }
        private readonly IActorRef _taskSchedulingBus;
        private IActorRef _pipelineInstance;
        private IActorRef _callingActor;
        public CodeRunner(int runnerId, IActorRef taskSchedulingBus)
        {
            RunnerId = runnerId;
            _taskSchedulingBus = taskSchedulingBus;
            Become(NormalState);
        }

        protected void NormalState()
        {
            Receive<RunnerStartMessage>(message =>
            {
                _pipelineInstance = Context.ActorOf(Props.Create(() => new WebPipelineInstanceActor(message.WorkspaceXml)), "pipelineInstance");
                _pipelineInstance.Tell(new InstanceStartMessage(message.AdditionalData), Self);
                _callingActor = Sender;
                Become(RunningState);
            });
        }

        protected void RunningState()
        {
            Receive<InstanceStartedMessage>(message =>
            {
                _callingActor.Tell(new RunnerStartedMessage(RunnerId));
            });
            Receive<InstanceFinishedMessage>(message =>
            {
                Context.Stop(_pipelineInstance);
                _taskSchedulingBus.Tell(new RunnerFinishedMessage(RunnerId, message.Result));
                Become(NormalState);
            });
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                maxNrOfRetries: 3,
                withinTimeRange: TimeSpan.FromSeconds(30),
                localOnlyDecider: e => Directive.Stop);
        }
    }
}