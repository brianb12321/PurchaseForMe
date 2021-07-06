using System;
using AngleSharp.Dom;
using IronBlock;
using IronBlock.Blocks;

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

            IElement rootElement;
            if (fromElement)
            {
                rootElement = (IElement)Values.Evaluate("rootElement", context);
            }
            else
            {
                IDocument currentPage = (IDocument) context.GetRootContext().Variables["__currentWebPage"];
                rootElement = currentPage.DocumentElement;
            } 
            IElement element = null;
            switch (type)
            {
                case ElementType.Class:
                    element = rootElement.QuerySelector($".{name}");
                    break;
                case ElementType.Id:
                    element = rootElement.QuerySelector($"#{name}");
                    break;
                case ElementType.Name:
                case ElementType.TagName:
                case ElementType.CssSelector:
                    element = rootElement.QuerySelector(name);
                    break;
            }

            return element;
        }
    }
}