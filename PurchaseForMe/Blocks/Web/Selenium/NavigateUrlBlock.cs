using IronBlock;
using IronBlock.Blocks;
using OpenQA.Selenium;

namespace PurchaseForMe.Blocks.Web.Selenium
{
    [RegisterBlock("web_navigateUrl", Category = "Selenium")]
    public class NavigateUrlBlock : IBlock
    {
        public override object Evaluate(Context context)
        {
            string url = Values.Evaluate("url", context).ToString();
            IWebDriver driver = (IWebDriver)context.GetRootContext().Variables["__driver"];
            driver.Navigate().GoToUrl(url);
            base.Evaluate(context);
            return null;
        }
    }
}
