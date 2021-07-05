using System;
using System.Linq;
using IronBlock;
using IronBlock.Blocks;
using OpenQA.Selenium;

namespace PurchaseForMe.Blocks.Web.Selenium
{
    [RegisterBlock("web_getSubElements", Category = "Selenium")]
    public class GetSubElementBlock : IBlock
    {
        public override object Evaluate(Context context)
        {
            ElementType type = Enum.Parse<ElementType>(this.Fields.Get("elementType"));
            bool fromElement = (Fields.Get("from") == "Element");
            string name = this.Values.Evaluate("elementName", context).ToString();
            if (!context.GetRootContext().Variables.ContainsKey("__driver"))
                throw new Exception("A web-driver has not been initialized. Please open a web driver.");

            ISearchContext rootElement;
            if (fromElement)
            {
                rootElement = (IWebElement)Values.Evaluate("rootElement", context);
            }
            else
            {
                rootElement = (IWebDriver)context.GetRootContext().Variables["__driver"];
            }
            IWebElement[] elements = null;
            switch (type)
            {
                case ElementType.Class:
                    elements = rootElement.FindElements(By.ClassName(name)).ToArray();
                    break;
                case ElementType.Id:
                    elements = rootElement.FindElements(By.Id(name)).ToArray();
                    break;
                case ElementType.Name:
                    elements = rootElement.FindElements(By.Name(name)).ToArray();
                    break;
                case ElementType.TagName:
                    elements = rootElement.FindElements(By.TagName(name)).ToArray();
                    break;
            }
            return elements;
        }
    }
}