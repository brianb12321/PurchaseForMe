using AngleSharp.Dom;
using IronBlock;
using IronBlock.Blocks;

namespace PurchaseForMe.Blocks.Web.AngleSharp
{
    [RegisterBlock("web_clickElement", Category = "AngleSharp")]
    public class ClickElementBlock : IBlock
    {
        public override object Evaluate(Context context)
        {
            IElement element = (IElement) Values.Evaluate("element", context);
            element.FireSimpleEvent("click");
            base.Evaluate(context);
            return null;
        }
    }
}