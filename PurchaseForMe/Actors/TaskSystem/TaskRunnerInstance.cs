using System;
using System.Linq;
using System.Reflection;
using Akka.Actor;
using IronBlock;
using IronBlock.Blocks;
using PurchaseForMe.Blocks;
using PurchaseForMe.Core.Code.Instance;
using PurchaseForMe.Core.WebPipeline;

namespace PurchaseForMe.Actors.TaskSystem
{
    public class TaskRunnerInstance : ReceiveActor
    {
        public string WorkspaceXml { get; }
        public TaskRunnerInstance(string workspaceXml)
        {
            WorkspaceXml = workspaceXml;
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
                    Workspace blockWorkspace = blockParser.Parse(WorkspaceXml);

                    //Redirect standard out to SignalR channel--Selenium logs to standard out.
                    //TextWriter old = Console.Out;
                    //SignalRTextWriter newWriter = new SignalRTextWriter(Clients.Caller);
                    //Console.SetOut(newWriter);
                    Sender.Tell(new InstanceStartedMessage());
                    blockWorkspace.Evaluate();
                    //Restore standard out
                    //Console.SetOut(old);
                    base.Sender.Tell(new InstanceFinishedMessage(null));
                }
                catch (Exception e)
                {
                    Sender.Tell(new InstanceFinishedMessage(null));
                }
            });
        }
    }
}