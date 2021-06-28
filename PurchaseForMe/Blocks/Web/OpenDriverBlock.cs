using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IronBlock;
using IronBlock.Blocks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace PurchaseForMe.Blocks.Web
{
    [RegisterBlock("web_openDriver")]
    public class OpenDriverBlock : IBlock
    {
        public enum DriverType
        {
            Chrome
        }
        public override object Evaluate(Context context)
        {
            DriverType driverType = Enum.Parse<DriverType>(this.Fields.Get("driverType"));
            string url = this.Values.Evaluate("url", context).ToString();
            Statement body = this.Statements.Get("body");
            IWebDriver driver = null;
            try
            {
                switch (driverType)
                {
                    default:
                    case DriverType.Chrome:
                        driver = new ChromeDriver();
                        break;
                }

                driver.Navigate().GoToUrl(url);
                context.Variables.Add("Driver", driver);
                body.Evaluate(context);
            }
            finally
            {
                driver?.Close();
            }

            return null;
        }
    }
}