using IronBlock;
using IronBlock.Blocks;
using OpenQA.Selenium;

namespace PurchaseForMeService.Blocks.Web.Selenium
{
    [RegisterBlock("web_maximizeWindow", Category = "Selenium")]
    public class MaximizeWindowBlock : IBlock
    {
        public override object Evaluate(Context context)
        {
            IWebDriver driver = (IWebDriver) context.GetRootContext().Variables["__driver"];
            driver.Manage().Window.Maximize();
            base.Evaluate(context);
            return null;
        }
    }
}