using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IronBlock;
using IronBlock.Blocks;
using OpenQA.Selenium;

namespace PurchaseForMe.Blocks.Web
{
    [RegisterBlock("web_maximizeWindow")]
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