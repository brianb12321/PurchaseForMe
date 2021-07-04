using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IronBlock;
using IronBlock.Blocks;
using OpenQA.Selenium;

namespace PurchaseForMe.Blocks.Web
{
    [RegisterBlock("web_getElementAttribute")]
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