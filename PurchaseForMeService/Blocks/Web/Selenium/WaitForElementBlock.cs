using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronBlock;
using IronBlock.Blocks;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
#pragma warning disable 618

namespace PurchaseForMeService.Blocks.Web.Selenium
{
    [RegisterBlock("web_waitForElement", Category = "Selenium")]
    public class WaitForElementBlock : IBlock
    {
        public enum WaitType
        {
            Load, Click
        }
        public override object Evaluate(Context context)
        {
            ElementType type = Enum.Parse<ElementType>(this.Fields.Get("elementType"));
            WaitType waitType = Enum.Parse<WaitType>(this.Fields.Get("waitType"));
            string name = this.Values.Evaluate("elementName", context).ToString();
            if (!context.GetRootContext().Variables.ContainsKey("__driver"))
                throw new Exception("A web-driver has not been initialized. Please open a web driver.");

            IWebDriver driver = (IWebDriver)context.GetRootContext().Variables["__driver"];
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            switch (type)
            {
                case ElementType.Class:
                    if (waitType == WaitType.Click)
                    {
                        wait.Until(ExpectedConditions.ElementToBeClickable(By.ClassName(name)));
                    }
                    else
                    {
                        wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName(name)));
                    }
                    
                    break;
                case ElementType.Id:
                    if (waitType == WaitType.Click)
                    {
                        wait.Until(ExpectedConditions.ElementToBeClickable(By.Id(name)));
                    }
                    else
                    {
                        wait.Until(ExpectedConditions.ElementIsVisible(By.Id(name)));
                    }
                    break;
                case ElementType.Name:
                    if (waitType == WaitType.Click)
                    {
                        wait.Until(ExpectedConditions.ElementToBeClickable(By.Name(name)));
                    }
                    else
                    {
                        wait.Until(ExpectedConditions.ElementIsVisible(By.Name(name)));
                    }
                    break;
                case ElementType.TagName:
                    if (waitType == WaitType.Click)
                    {
                        wait.Until(ExpectedConditions.ElementToBeClickable(By.TagName(name)));
                    }
                    else
                    {
                        wait.Until(ExpectedConditions.ElementIsVisible(By.TagName(name)));
                    }
                    break;
                case ElementType.CssSelector:
                    if (waitType == WaitType.Click)
                    {
                        wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(name)));
                    }
                    else
                    {
                        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(name)));
                    }
                    break;
                case ElementType.XPath:
                    if (waitType == WaitType.Click)
                    {
                        wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(name)));
                    }
                    else
                    {
                        wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(name)));
                    }
                    break;
            }

            base.Evaluate(context);
            return null;
        }
    }
}