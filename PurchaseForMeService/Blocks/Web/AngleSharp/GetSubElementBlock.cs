using System;
using System.Linq;
using AngleSharp.Dom;
using HtmlAgilityPack;
using IronBlock;
using IronBlock.Blocks;
using ScrapySharp.Extensions;
using ScrapySharp.Network;

namespace PurchaseForMeService.Blocks.Web.AngleSharp
{
    [RegisterBlock("web_getSubElements", Category = "AngleSharp")]
    public class GetSubElementBlock : IBlock
    {
        public override object Evaluate(Context context)
        {
            ElementType type = Enum.Parse<ElementType>(this.Fields.Get("elementType"));
            bool fromElement = (Fields.Get("from") == "Element");
            string name = this.Values.Evaluate("elementName", context).ToString();
            if (!context.GetRootContext().Variables.ContainsKey("__currentWebPage"))
                throw new Exception("No webpage has been loaded yet.");

            HtmlNode rootElement;
            if (fromElement)
            {
                rootElement = (HtmlNode)Values.Evaluate("rootElement", context);
            }
            else
            {
                WebPage currentPage = (WebPage)context.GetRootContext().Variables["__currentWebPage"];
                rootElement = currentPage.Html;
            }
            HtmlNode[] elements = null;
            switch (type)
            {
                case ElementType.Class:
                    elements = rootElement.CssSelect($".{name}").ToArray();
                    break;
                case ElementType.Id:
                    elements = rootElement.CssSelect($"#{name}").ToArray();
                    break;
                case ElementType.Name:
                case ElementType.TagName:
                case ElementType.CssSelector:
                    elements = rootElement.CssSelect(name).ToArray();
                    break;
            }
            return elements;
        }
    }
}