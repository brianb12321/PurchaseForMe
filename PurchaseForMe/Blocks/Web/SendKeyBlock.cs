using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IronBlock;
using IronBlock.Blocks;
using OpenQA.Selenium;

namespace PurchaseForMe.Blocks.Web
{
    [RegisterBlock("web_sendKey")]
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