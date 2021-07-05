using System;
using AngleSharp.Dom;
using IronBlock;
using IronBlock.Blocks;
using OpenQA.Selenium;

namespace PurchaseForMe.Blocks.Web.AngleSharp
{
    [RegisterBlock("web_getElementDetail", Category = "AngleSharp")]
    public class GetElementDetailBlock : IBlock
    {
        public override object Evaluate(Context context)
        {
            ElementInformationType infoType = Enum.Parse<ElementInformationType>(this.Fields.Get("informationType"));
            IElement element = (IElement)this.Values.Evaluate("element", context);
            switch (infoType)
            {
                default:
                case ElementInformationType.InnerHtml:
                    string innerHtml = element.InnerHtml;
                    return innerHtml;
                case ElementInformationType.InnerText:
                    string innerText = element.Text();
                    return innerText;
            }
        }
    }
}