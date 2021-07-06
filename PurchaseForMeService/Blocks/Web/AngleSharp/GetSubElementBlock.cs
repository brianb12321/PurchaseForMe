using System;
using System.Linq;
using AngleSharp.Dom;
using IronBlock;
using IronBlock.Blocks;

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

            IElement rootElement;
            if (fromElement)
            {
                rootElement = (IElement)Values.Evaluate("rootElement", context);
            }
            else
            {
                IDocument currentPage = (IDocument)context.GetRootContext().Variables["__currentWebPage"];
                rootElement = currentPage.DocumentElement;
            }
            IElement[] elements = null;
            switch (type)
            {
                case ElementType.Class:
                    elements = rootElement.QuerySelectorAll($".{name}").ToArray();
                    break;
                case ElementType.Id:
                    elements = rootElement.QuerySelectorAll($"#{name}").ToArray();
                    break;
                case ElementType.Name:
                case ElementType.TagName:
                case ElementType.CssSelector:
                    elements = rootElement.QuerySelectorAll(name).ToArray();
                    break;
            }
            return elements;
        }
    }
}