using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IronBlock;
using IronBlock.Blocks;
using OpenQA.Selenium;

namespace PurchaseForMe.Blocks.Web
{
    [RegisterBlock("web_clickElement")]
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