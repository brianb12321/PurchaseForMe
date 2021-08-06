using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using Akka.Actor;
using IronBlock;
using PurchaseForMe;
using PurchaseForMe.Core;
using PurchaseForMe.Core.Code.Abstraction;
using PurchaseForMe.Core.Code.Instance;
using PurchaseForMe.Core.Project;
using PurchaseForMe.Core.TaskSystem;
using PurchaseForMeService.Blocks;

namespace PurchaseForMeService.Actors.TaskSystem
{
    public class TaskRunnerInstance : ReceiveActor
    {
        public TaskRunnerInstance(IActorRef pipelineBus, ICodeContextFactory factory)
        {
            Receive<InstanceStartMessage>(message =>
            {
                //Parser blockParser = new Parser();
                //blockParser.AddStandardBlocksEx();
                //Will move later.

                try
                {
                    ProjectInstance project = ((TaskStartMessage) message.AdditionalData).Project;
                    TaskStartMessage startMessage = (TaskStartMessage)message.AdditionalData;
                    
                    CodeChannelWriter channelWriter =
                        new CodeChannelWriter(startMessage.TaskNode.NodeGuid, Context.System.EventStream);

                    ICodeContext context = factory.Create(message.WorkspaceXml);
                    context.Variables.Add("__standardOut", channelWriter);
                    context.Variables.Add("__currentProject", project);
                    context.Variables.Add("__pipelineSchedulingBus", pipelineBus);
                    Sender.Tell(new InstanceStartedMessage());
                    context.Execute(null, CancellationToken.None);
                    TaskCompleted completed =
                        new TaskCompleted(startMessage.TaskNode.NodeGuid, true, "Task completed successfully");

                    base.Sender.Tell(new InstanceFinishedMessage(completed));
                }
                catch (Exception e)
                {
                    TaskStartMessage startMessage = (TaskStartMessage) message.AdditionalData;
                    TaskCompleted completed = new TaskCompleted(startMessage.TaskNode.NodeGuid, false,
                        $"Task completed with error: {e.ToString()}");
                    Sender.Tell(new InstanceFinishedMessage(completed));
                }
            });
        }
    }
}