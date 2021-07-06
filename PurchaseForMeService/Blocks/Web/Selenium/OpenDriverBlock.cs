using System;
using IronBlock;
using IronBlock.Blocks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace PurchaseForMeService.Blocks.Web.Selenium
{
    [RegisterBlock("web_openDriver", Category = "Selenium")]
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
            Statement setupBody = null;
            try
            {
                setupBody = this.Statements.Get("setupBody");
            }
            catch (ArgumentException)
            {

            }
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
                context.GetRootContext().Variables.Add("__driver", driver);
                if (setupBody != null)
                {
                    setupBody.Evaluate(context);
                }

                driver.Navigate().GoToUrl(url);
                body.Evaluate(context);
            }
            finally
            {
                driver?.Quit();
                base.Evaluate(context);
            }

            return null;
        }
    }
}