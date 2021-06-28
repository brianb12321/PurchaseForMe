using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IronBlock;
using IronBlock.Blocks;
using Newtonsoft.Json.Linq;
using PurchaseForMe.Core.WebPipeline;

namespace PurchaseForMe.Blocks.Pipeline
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