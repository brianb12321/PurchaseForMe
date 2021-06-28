using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IronBlock;
using IronBlock.Blocks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PurchaseForMe.Core.WebPipeline;

namespace PurchaseForMe.Blocks.Pipeline
{
    [RegisterBlock("pipeline_createWebModel")]
    public class CreateWebDataModelBlock : IBlock
    {
        public override object Evaluate(Context context)
        {
            object valueObject = this.Values.Evaluate("object", context);
            WebDataModel model = new WebDataModel();
            model.ModelData = valueObject;
            return model;
        }
    }
}