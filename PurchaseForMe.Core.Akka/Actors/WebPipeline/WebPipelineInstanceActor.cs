using System;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Reflection;
using Akka.Actor;
using AngleSharp;
using IronBlock;
using IronBlock.Blocks;
using PurchaseForMe.Blocks;
using PurchaseForMe.Core.WebPipeline;
using Microsoft.CodeAnalysis.Syntax;
using PurchaseForMe.Core.Code.Instance;
using PurchaseForMe.Hubs;

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
                    if (block != null && (block.Category is "All" or "AngleSharp"))
                    {
                        blockParser.AddBlock(block.BlockName, () => (IBlock)Activator.CreateInstance(type));
                    }
                }

                IBrowsingContext browsingContext = null;
                try
                {
                    PipelineRunRequest originalMessage = (PipelineRunRequest)r.AdditionalData;
                    Workspace blockWorkspace = blockParser.Parse(r.WorkspaceXml);

                    //Setup variables
                    IConfiguration webConfiguration = AngleSharp.Configuration.Default
                        .WithDefaultLoader()
                        .WithDefaultCookies();

                    browsingContext = new BrowsingContext(webConfiguration);
                    Context globalContext = new Context();
                    CodeChannelWriter channelWriter = new CodeChannelWriter(originalMessage.PipelineNode.NodeGuid,
                        Context.System.EventStream);
                    globalContext.Variables.Add("__browsingContext", browsingContext);
                    globalContext.Variables.Add("__standardOut", channelWriter);
                    Sender.Tell(new InstanceStartedMessage());
                    WebDataModel model = blockWorkspace.Evaluate(globalContext) as WebDataModel;

                    PipelineInstanceResult result = new PipelineInstanceResult();
                    result.IsSuccessful = true;
                    result.WebDataModel = model;
                    result.CodeGuid = originalMessage.PipelineNode.NodeGuid;
                    result.ResultMessage = "Pipeline ran successfully";

                    base.Sender.Tell(new InstanceFinishedMessage(result));
                }
                catch (Exception e)
                {
                    dynamic errorObject = new
                    {
                        ErrorMessage = e.Message
                    };
                    PipelineInstanceResult result = new PipelineInstanceResult();
                    PipelineRunRequest originalMessage = (PipelineRunRequest) r.AdditionalData;
                    result.IsSuccessful = false;
                    result.WebDataModel = new WebDataModel();
                    result.WebDataModel.ModelData = errorObject;
                    result.CodeGuid = originalMessage.PipelineNode.NodeGuid;
                    result.ResultMessage = "Pipeline ran with error(s).";
                    Sender.Tell(new InstanceFinishedMessage(result));
                }
                finally
                {
                    browsingContext?.Dispose();
                }
            });
        }
    }
}