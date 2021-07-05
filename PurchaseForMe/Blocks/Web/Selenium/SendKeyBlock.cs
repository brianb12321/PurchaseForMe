using IronBlock;
using IronBlock.Blocks;
using OpenQA.Selenium;

namespace PurchaseForMe.Blocks.Web.Selenium
{
    [RegisterBlock("web_sendKey", Category = "Selenium")]
    public class SendKeyBlock : IBlock
    {
        public override object Evaluate(Context context)
        {
            IWebElement element = (IWebElement)Values.Evaluate("element", context);
            string keyString = (string)Values.Evaluate("keyString", context);
            bool shouldIncludeEnterKey = bool.Parse(Fields.Get("shouldSendEnterKey"));
            element.SendKeys($"{keyString}{(shouldIncludeEnterKey ? Keys.Enter : string.Empty)}");
            base.Evaluate(context);
            return null;
        }
    }
}