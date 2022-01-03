using System.Threading.Tasks;
using Akka.Actor;
using PurchaseForMe.Core.Code.Abstraction;
using PurchaseForMe.Core.Code.Instance;
using PurchaseForMe.Core.Project;
using PurchaseForMe.Core.TaskSystem;

namespace PurchaseForMeService.Actors.TaskSystem
{
    public class TaskRunnerInstance : CodeInstanceActor
    {
        private readonly IActorRef _pipelineSchedulingBus;

        public TaskRunnerInstance(IActorRef pipelineBus, ICodeContextFactory factory) : base(factory)
        {
            _pipelineSchedulingBus = pipelineBus;
        }

        protected override Task ConfigureCodeContext(ICodeContext context, InstanceStartMessage message)
        {
            ProjectInstance project = ((TaskStartMessage)message.AdditionalData).Project;
            context.Variables.Add("__currentProject", project);
            context.Variables.Add("__pipelineSchedulingBus", _pipelineSchedulingBus);
            return Task.CompletedTask;
        }
    }
}