using IronBlock;
using IronBlock.Blocks;
using OpenQA.Selenium;

namespace PurchaseForMe.Blocks.Web.Selenium
{
    [RegisterBlock("web_getElementAttribute", Category = "Selenium")]
    public class GetElementAttributeBlock : IBlock
    {
        public override object Evaluate(Context context)
        {
            string attributeName = (string)Values.Evaluate("attributeName", context);
            IWebElement element = (IWebElement)this.Values.Evaluate("element", context);
            return element.GetAttribute(attributeName);
        }
    }
}