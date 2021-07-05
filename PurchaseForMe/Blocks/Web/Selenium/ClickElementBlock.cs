using IronBlock;
using IronBlock.Blocks;
using OpenQA.Selenium;

namespace PurchaseForMe.Blocks.Web.Selenium
{
    [RegisterBlock("web_clickElement", Category = "Selenium")]
    public class ClickElementBlock : IBlock
    {
        public override object Evaluate(Context context)
        {
            IWebElement element = (IWebElement)Values.Evaluate("element", context);
            element.Click();
            base.Evaluate(context);
            return null;
        }
    }
}