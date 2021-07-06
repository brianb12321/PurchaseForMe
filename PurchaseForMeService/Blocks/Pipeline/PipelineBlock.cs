using IronBlock;
using IronBlock.Blocks;
using PurchaseForMe.Core.WebPipeline;

namespace PurchaseForMeService.Blocks.Pipeline
{
    [RegisterBlock("pipeline_pipelineBody")]
    public class PipelineBlock : IBlock
    {
        public override object Evaluate(Context context)
        {
            Statement pipelineBody = this.Statements.Get("body");
            pipelineBody.Evaluate(context);
            WebDataModel model = this.Values.Evaluate("webDataModel", context) as WebDataModel;
            return model;
        }
    }
}