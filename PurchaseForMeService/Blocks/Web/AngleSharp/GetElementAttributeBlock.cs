using AngleSharp.Dom;
using IronBlock;
using IronBlock.Blocks;

namespace PurchaseForMeService.Blocks.Web.AngleSharp
{
    [RegisterBlock("web_getElementAttribute", Category = "AngleSharp")]
    public class GetElementAttributeBlock : IBlock
    {
        public override object Evaluate(Context context)
        {
            string attributeName = (string)Values.Evaluate("attributeName", context);
            IElement element = (IElement)Values.Evaluate("element", context);
            return element.GetAttribute(attributeName);
        }
    }
}