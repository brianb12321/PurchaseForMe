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
    

    [RegisterBlock("web_getElement", Category = "AngleSharp")]
    public class GetElementBlock : IBlock
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
                WebPage currentPage = (WebPage) context.GetRootContext().Variables["__currentWebPage"];
                rootElement = currentPage.Html;
            } 
            HtmlNode element = null;
            switch (type)
            {
                case ElementType.Class:
                    element = rootElement.CssSelect($".{name}").First();
                    break;
                case ElementType.Id:
                    element = rootElement.CssSelect($"#{name}").First();
                    break;
                case ElementType.Name:
                case ElementType.TagName:
                case ElementType.CssSelector:
                    element = rootElement.CssSelect(name).First();
                    break;
            }

            return element;
        }
    }
}