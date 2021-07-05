using System;
using System.Linq;
using System.Reflection;
using Akka.Actor;
using IronBlock;
using IronBlock.Blocks;
using PurchaseForMe.Blocks;
using PurchaseForMe.Core.WebPipeline;
using Microsoft.CodeAnalysis.Syntax;
using PurchaseForMe.Core.Code.Instance;

namespace PurchaseForMe.Actors.WebPipeline
{
    public class WebPipelineInstanceActor : ReceiveActor
    {
        public WebPipelineInstanceActor()
        {
            Receive<InstanceStartMessage>(r =>
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
                    Workspace blockWorkspace = blockParser.Parse(r.WorkspaceXml);

                    //Redirect standard out to SignalR channel--Selenium logs to standard out.
                    //TextWriter old = Console.Out;
                    //SignalRTextWriter newWriter = new SignalRTextWriter(Clients.Caller);
                    //Console.SetOut(newWriter);
                    Sender.Tell(new InstanceStartedMessage());
                    WebDataModel model = blockWorkspace.Evaluate() as WebDataModel;
                    //Restore standard out
                    //Console.SetOut(old);

                    PipelineInstanceResult result = new PipelineInstanceResult();
                    PipelineRunRequest originalMessage = (PipelineRunRequest) r.AdditionalData;
                    result.IsSuccessful = true;
                    result.WebDataModel = model;
                    result.SessionId = originalMessage.SessionId;
                    if (originalMessage.ReturnCode)
                    {
                        
                    }
                    base.Sender.Tell(new InstanceFinishedMessage(result));
                }
                catch (Exception e)
                {
                    dynamic errorObject = new
                    {
                        ErrorMessage = e.Message
                    };
                    PipelineInstanceResult result = new PipelineInstanceResult();
                    PipelineRunRequest originalMessage = (PipelineRunRequest)r.AdditionalData;
                    result.IsSuccessful = false;
                    result.WebDataModel = new WebDataModel();
                    result.WebDataModel.ModelData = errorObject;
                    result.SessionId = originalMessage.SessionId;
                    Sender.Tell(new InstanceFinishedMessage(result));
                }
            });
        }
    }
}