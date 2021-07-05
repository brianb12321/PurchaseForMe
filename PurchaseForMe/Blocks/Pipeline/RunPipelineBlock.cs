using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using IronBlock;
using IronBlock.Blocks;
using PurchaseForMe.Core.Project;
using PurchaseForMe.Core.Project.Nodes.Blockly;
using PurchaseForMe.Core.WebPipeline;

namespace PurchaseForMe.Blocks.Pipeline
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
            request.WorkspaceXml = ((BlocklyPipelineNode)project.ProjectItems.First(i =>
                i.NodeType == NodeType.BlocklyPipeline && ((BlocklyPipelineNode) i).CodeName == pipelineCodeName)).BlocklyWorkspace.InnerXml;

            PipelineInstanceResult result = pipelineSchedulingBus.Ask<PipelineInstanceResult>(request).GetAwaiter().GetResult();
            return result.WebDataModel;
        }
    }
}