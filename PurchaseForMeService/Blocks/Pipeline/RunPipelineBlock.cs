using System.Linq;
using Akka.Actor;
using IronBlock;
using IronBlock.Blocks;
using PurchaseForMe.Core.Project;
using PurchaseForMe.Core.Project.Nodes.Blockly;
using PurchaseForMe.Core.WebPipeline;

namespace PurchaseForMeService.Blocks.Pipeline
{
    [RegisterBlock("pipeline_runPipeline")]
    public class RunPipelineBlock : IBlock
    {
        public override object Evaluate(Context context)
        {
            string pipelineCodeName = Values.Evaluate("pipelineCodeName", context).ToString();
            Context globalContext = context.GetRootContext();
            IActorRef pipelineSchedulingBus = (IActorRef)globalContext.Variables["__pipelineSchedulingBus"];
            PipelineRunRequest request = new PipelineRunRequest();
            ProjectInstance project = (ProjectInstance)globalContext.Variables["__currentProject"];
            request.PipelineNode = ((BlocklyPipelineNode)project.ProjectItems.First(i =>
                i.NodeType == NodeType.BlocklyPipeline && ((BlocklyPipelineNode) i).CodeName == pipelineCodeName));

            PipelineInstanceResult result = pipelineSchedulingBus.Ask<PipelineInstanceResult>(request).GetAwaiter().GetResult();
            return result.WebDataModel;
        }
    }
}