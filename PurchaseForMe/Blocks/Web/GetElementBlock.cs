using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IronBlock;
using IronBlock.Blocks;
using OpenQA.Selenium;

namespace PurchaseForMe.Blocks.Web
{
    public enum ElementType
    {
        Id, Class, Name, TagName
    }

    [RegisterBlock("web_getElement")]
    public class GetElementBlock : IBlock
    {
        
        public override object Evaluate(Context context)
        {
            ElementType type = Enum.Parse<ElementType>(this.Fields.Get("elementType"));
            string name = this.Values.Evaluate("elementName", context).ToString();
            if (!context.Variables.ContainsKey("Driver"))
                throw new Exception("A web-driver has not been initialized. Please open a web driver.");

            IWebDriver driver = (IWebDriver)context.Variables["Driver"];
            IWebElement element = null;
            switch (type)
            {
                case ElementType.Class:
                    element = driver.FindElement(By.ClassName(name));
                    break;
                case ElementType.Id:
                    element = driver.FindElement(By.Id(name));
                    break;
                case ElementType.Name:
                    element = driver.FindElement(By.Name(name));
                    break;
                case ElementType.TagName:
                    element = driver.FindElement(By.TagName(name));
                    break;
            }

            return element;
        }
    }
}