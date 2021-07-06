using System;
using IronBlock;
using IronBlock.Blocks;
using OpenQA.Selenium;

namespace PurchaseForMeService.Blocks.Web.Selenium
{
    [RegisterBlock("web_getElement", Category = "Selenium")]
    public class GetElementBlock : IBlock
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
            IWebElement element = null;
            switch (type)
            {
                case ElementType.Class:
                    element = rootElement.FindElement(By.ClassName(name));
                    break;
                case ElementType.Id:
                    element = rootElement.FindElement(By.Id(name));
                    break;
                case ElementType.Name:
                    element = rootElement.FindElement(By.Name(name));
                    break;
                case ElementType.TagName:
                    element = rootElement.FindElement(By.TagName(name));
                    break;
                case ElementType.CssSelector:
                    element = rootElement.FindElement(By.CssSelector(name));
                    break;
            }

            return element;
        }
    }
}