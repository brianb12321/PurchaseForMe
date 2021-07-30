using System;
using Akka.Actor;
using PurchaseForMe.Core.Code.Instance;
using PurchaseForMe.Core.Code.Runner;
using PurchaseForMe.Core.TaskSystem.TaskRunner;

namespace PurchaseForMeService.Actors
{
    public class CodeRunner<TInstanceActor> : ReceiveActor where TInstanceActor : ActorBase
    {
        public int RunnerId { get; }
        private readonly IActorRef _taskSchedulingBus;
        private IActorRef _pipelineInstance;
        private IActorRef _callingActor;
        private readonly Func<object[]> _parameterProvider;
        private readonly string _runnerInstanceName;
        public CodeRunner(int runnerId, IActorRef taskSchedulingBus, Func<object[]> parameterProvider, string runnerInstanceName)
        {
            RunnerId = runnerId;
            _runnerInstanceName = runnerInstanceName;
            _parameterProvider = parameterProvider;
            _taskSchedulingBus = taskSchedulingBus;
            Become(NormalState);
        }

        protected void NormalState()
        {
            Receive<RunnerStartMessage>(message =>
            {
                _pipelineInstance = Context.ActorOf(Props.Create<TInstanceActor>(_parameterProvider()), _runnerInstanceName);
                _pipelineInstance.Tell(new InstanceStartMessage(message.WorkspaceXml, message.AdditionalData), Self);
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