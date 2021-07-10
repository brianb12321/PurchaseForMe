using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronBlock;
using IronBlock.Blocks;
using OpenQA.Selenium;

namespace PurchaseForMeService.Blocks.Web.Selenium
{
    [RegisterBlock("web_refresh", Category = "Selenium")]
    public class RefreshBlock : IBlock
    {
        public override object Evaluate(Context context)
        {
            IWebDriver driver = (IWebDriver)context.GetRootContext().Variables["__driver"];
            driver.Navigate().Refresh();
            base.Evaluate(context);
            return null;
        }
    }
}
