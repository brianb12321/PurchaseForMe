using AngleSharp.Dom;
using HtmlAgilityPack;
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
            HtmlNode element = (HtmlNode)Values.Evaluate("element", context);
            return element.GetAttributeValue(attributeName, "undefined");
        }
    }
}