using System;
using System.Linq;
using System.Reflection;
using Akka.Actor;
using IronBlock;
using PurchaseForMe;
using PurchaseForMe.Core;
using PurchaseForMe.Core.Code.Instance;
using PurchaseForMe.Core.Project;
using PurchaseForMe.Core.TaskSystem;
using PurchaseForMeService.Blocks;

namespace PurchaseForMeService.Actors.TaskSystem
{
    public class TaskRunnerInstance : ReceiveActor
    {
        public TaskRunnerInstance(IActorRef pipelineBus)
        {
            Receive<InstanceStartMessage>(message =>
            {
                Parser blockParser = new Parser();
                blockParser.AddStandardBlocksEx();
                //Will move later.
                Assembly assembly = Assembly.GetExecutingAssembly();
                foreach (Type type in assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract))
                {
                    RegisterBlockAttribute block = type.GetCustomAttribute<RegisterBlockAttribute>();
                    if (block != null && (block.Category is "All" or "Selenium"))
                    {
                        blockParser.AddBlock(block.BlockName, () => (IBlock)Activator.CreateInstance(type));
                    }
                }

                try
                {
                    ProjectInstance project = ((TaskStartMessage) message.AdditionalData).Project;
                    TaskStartMessage startMessage = (TaskStartMessage)message.AdditionalData;
                    Workspace blockWorkspace = blockParser.Parse(message.WorkspaceXml);

                    //Setup variables; setup Selenium
                    Context rootContext = new Context();
                    CodeChannelWriter channelWriter =
                        new CodeChannelWriter(startMessage.TaskNode.NodeGuid, Context.System.EventStream);
                    rootContext.Variables.Add("__standardOut", channelWriter);
                    rootContext.Variables.Add("__currentProject", project);
                    rootContext.Variables.Add("__pipelineSchedulingBus", pipelineBus);
                    Sender.Tell(new InstanceStartedMessage());
                    blockWorkspace.Evaluate(rootContext);
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