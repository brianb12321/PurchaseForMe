using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IronBlock;
using IronBlock.Blocks;
using OpenQA.Selenium;

namespace PurchaseForMe.Blocks.Web
{
    [RegisterBlock("web_getElementDetail")]
    public class GetElementDetailBlock : IBlock
    {
        public enum ElementInformationType
        {
            InnerHtml
        }
        public override object Evaluate(Context context)
        {
            ElementInformationType infoType = Enum.Parse<ElementInformationType>(this.Fields.Get("informationType"));
            IWebElement element = (IWebElement)this.Values.Evaluate("element", context);
            IWebDriver driver = (IWebDriver)context.Variables["Driver"];
            switch (infoType)
            {
                default:
                case ElementInformationType.InnerHtml:
                    if (driver is IJavaScriptExecutor js)
                    {
                        string innerHtml = (string) js.ExecuteScript("return arguments[0].innerHTML;", element);
                        return innerHtml;
                    }
                    else throw new ArgumentException("Driver is not a javascript executor.");
            }
        }
    }
}