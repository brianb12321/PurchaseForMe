using System;
using System.Linq;
using System.Reflection;
using Akka.Actor;
using IronBlock;
using IronBlock.Blocks;
using PurchaseForMe.Actors.WebPipeline;
using PurchaseForMe.Blocks;
using PurchaseForMe.Core.Code.Instance;
using PurchaseForMe.Core.Project;
using PurchaseForMe.Core.TaskSystem;
using PurchaseForMe.Core.WebPipeline;

namespace PurchaseForMe.Actors.TaskSystem
{
    public class TaskRunnerInstance : ReceiveActor
    {
        public TaskRunnerInstance(PipelineSchedulingBusFactory pipelineBus)
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
                    if (block != null)
                    {
                        blockParser.AddBlock(block.BlockName, () => (IBlock)Activator.CreateInstance(type));
                    }
                }

                try
                {
                    ProjectInstance project = ((TaskStartMessage)message.AdditionalData).Project;
                    Workspace blockWorkspace = blockParser.Parse(message.WorkspaceXml);

                    //Redirect standard out to SignalR channel--Selenium logs to standard out.
                    //TextWriter old = Console.Out;
                    //SignalRTextWriter newWriter = new SignalRTextWriter(Clients.Caller);
                    //Console.SetOut(newWriter);
                    Sender.Tell(new InstanceStartedMessage());
                    Context rootContext = new Context();
                    rootContext.Variables.Add("__currentProject", project);
                    rootContext.Variables.Add("__pipelineSchedulingBus", pipelineBus());
                    blockWorkspace.Evaluate(rootContext);
                    TaskStartMessage startMessage = (TaskStartMessage) message.AdditionalData;
                    TaskCompleted completed = new TaskCompleted(startMessage.SessionId, true, "Task completed successfully");

                    //Restore standard out
                    //Console.SetOut(old);
                    base.Sender.Tell(new InstanceFinishedMessage(completed));
                }
                catch (Exception e)
                {
                    TaskStartMessage startMessage = (TaskStartMessage)message.AdditionalData;
                    TaskCompleted completed = new TaskCompleted(startMessage.SessionId, true, $"Task completed with error: {e.Message}");
                    Sender.Tell(new InstanceFinishedMessage(completed));
                }
            });
        }
    }
}