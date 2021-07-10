using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronBlock;
using IronBlock.Blocks;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PurchaseForMeService.Blocks.Web.Selenium
{
    [RegisterBlock("web_waitPage", Category = "Selenium")]
    public class WaitPageBlock : IBlock
    {
        public override object Evaluate(Context context)
        {
            IWebDriver driver = (IWebDriver)context.GetRootContext().Variables["__driver"];
            IWait<IWebDriver> wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
            wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            base.Evaluate(context);
            return null;
        }
    }
}