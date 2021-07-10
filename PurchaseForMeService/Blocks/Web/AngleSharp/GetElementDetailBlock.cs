using System;
using AngleSharp.Dom;
using HtmlAgilityPack;
using IronBlock;
using IronBlock.Blocks;

namespace PurchaseForMeService.Blocks.Web.AngleSharp
{
    [RegisterBlock("web_getElementDetail", Category = "AngleSharp")]
    public class GetElementDetailBlock : IBlock
    {
        public override object Evaluate(Context context)
        {
            ElementInformationType infoType = Enum.Parse<ElementInformationType>(this.Fields.Get("informationType"));
            HtmlNode element = (HtmlNode)this.Values.Evaluate("element", context);
            switch (infoType)
            {
                default:
                case ElementInformationType.InnerHtml:
                    string innerHtml = element.InnerHtml;
                    return innerHtml;
                case ElementInformationType.InnerText:
                    string innerText = element.InnerText;
                    return innerText;
            }
        }
    }
}