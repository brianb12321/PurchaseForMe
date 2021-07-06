using IronBlock;
using IronBlock.Blocks;
using PurchaseForMe.Core.WebPipeline;

namespace PurchaseForMeService.Blocks.Pipeline
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