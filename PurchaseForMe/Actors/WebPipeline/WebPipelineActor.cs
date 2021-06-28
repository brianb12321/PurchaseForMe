using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using PurchaseForMe.Core.WebPipeline;

namespace PurchaseForMe.Actors.WebPipeline
{
    public delegate IActorRef WebPipelineActorFactory();
    public class WebPipelineActor : ReceiveActor
    {
        private IActorRef _callingActorRef;
        private IActorRef _pipelineInstance;
        public WebPipelineActor()
        {
            Receive<PipelineRunRequest>(request =>
            {
                _callingActorRef = Sender;
                _pipelineInstance = Context.ActorOf(Props.Create<WebPipelineInstanceActor>(), "pipelineInstance");
                _pipelineInstance.Tell(request, Self);
            });
            Receive<PipelineInstanceResult>(result =>
            {
                _callingActorRef.Tell(result, Self);
                _callingActorRef = null;
                Context.Stop(_pipelineInstance);
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