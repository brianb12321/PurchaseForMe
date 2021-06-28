using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IronBlock;
using IronBlock.Blocks;
using OpenQA.Selenium;

namespace PurchaseForMe.Blocks.Web
{
    [RegisterBlock("web_getSubElements")]
    public class GetSubElementBlock : IBlock
    {
        public override object Evaluate(Context context)
        {
            ElementType type = Enum.Parse<ElementType>(this.Fields.Get("elementType"));
            string name = this.Values.Evaluate("elementName", context).ToString();
            if (!context.Variables.ContainsKey("Driver"))
                throw new Exception("A web-driver has not been initialized. Please open a web driver.");

            IWebDriver driver = (IWebDriver)context.Variables["Driver"];
            IWebElement[] elements = null;
            switch (type)
            {
                case ElementType.Class:
                    elements = driver.FindElements(By.ClassName(name)).ToArray();
                    break;
                case ElementType.Id:
                    elements = driver.FindElements(By.Id(name)).ToArray();
                    break;
                case ElementType.Name:
                    elements = driver.FindElements(By.Name(name)).ToArray();
                    break;
                case ElementType.TagName:
                    elements = driver.FindElements(By.TagName(name)).ToArray();
                    break;
            }

            return elements;
        }
    }
}