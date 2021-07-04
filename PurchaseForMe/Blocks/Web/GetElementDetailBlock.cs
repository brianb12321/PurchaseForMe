using System;
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
            InnerHtml, InnerText
        }
        public override object Evaluate(Context context)
        {
            ElementInformationType infoType = Enum.Parse<ElementInformationType>(this.Fields.Get("informationType"));
            IWebElement element = (IWebElement)this.Values.Evaluate("element", context);
            IWebDriver driver = (IWebDriver)context.GetRootContext().Variables["__driver"];
            if (driver is IJavaScriptExecutor js)
            {
                switch (infoType)
                {
                    default:
                    case ElementInformationType.InnerHtml:
                        string innerHtml = (string)js.ExecuteScript("return arguments[0].innerHTML;", element);
                        return innerHtml;
                    case ElementInformationType.InnerText:
                        string innerText = (string) js.ExecuteScript("return arguments[0].innerText", element);
                        return innerText;
                }
            }
            else throw new ArgumentException("Driver is not a javascript executor.");
        }
    }
}