using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using Akka.Actor;
using AngleSharp;
using IronBlock;
using PurchaseForMe;
using PurchaseForMe.Core.Code.Abstraction;
using PurchaseForMe.Core.Code.Instance;
using PurchaseForMe.Core.WebPipeline;
using PurchaseForMeService.Blocks;
using ScrapySharp.Network;

namespace PurchaseForMeService.Actors.WebPipeline
{
    public class WebPipelineInstanceActor : ReceiveActor
    {
        public WebPipelineInstanceActor(ICodeContextFactory factory)
        {
            Receive<InstanceStartMessage>(r =>
            {
                //Parser blockParser = new Parser();
                //blockParser.AddStandardBlocksEx();
                //Will move later.
                //Assembly assembly = Assembly.GetExecutingAssembly();
                //foreach (Type type in assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract))
                //{
                //    RegisterBlockAttribute block = type.GetCustomAttribute<RegisterBlockAttribute>();
                //    if (block != null && (block.Category is "All" or "Selenium"))
                //    {
                //        blockParser.AddBlock(block.BlockName, () => (IBlock)Activator.CreateInstance(type));
                //    }
                //}

                //IBrowsingContext browsingContext = null;
                ScrapingBrowser browser;
                try
                {
                    PipelineRunRequest originalMessage = (PipelineRunRequest)r.AdditionalData;
                    //Workspace blockWorkspace = blockParser.Parse(r.WorkspaceXml);
                    ICodeContext codeContext = factory.Create(r.WorkspaceXml);
                    //browser = new ScrapingBrowser();
                    //Setup variables
                    //IConfiguration webConfiguration = AngleSharp.Configuration.Default
                    //    .WithDefaultLoader()
                    //    .WithJs()
                    //    .WithDefaultCookies();

                    //browsingContext = new BrowsingContext(webConfiguration);
                    //Context globalContext = new Context();
                    CodeChannelWriter channelWriter = new CodeChannelWriter(originalMessage.PipelineNode.NodeGuid,
                        Context.System.EventStream);
                    //globalContext.Variables.Add("__browsingContext", browser);
                    codeContext.Variables.Add("__standardOut", channelWriter);
                    Sender.Tell(new InstanceStartedMessage());
                    WebDataModel model = codeContext.Execute(null, CancellationToken.None).GetAwaiter().GetResult() as WebDataModel;

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
                    //browser = null;
                    //browsingContext?.Dispose();
                }
            });
        }
    }
}