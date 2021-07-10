using System;
using AngleSharp;
using AngleSharp.Dom;
using IronBlock;
using IronBlock.Blocks;
using ScrapySharp.Network;

namespace PurchaseForMeService.Blocks.Web.AngleSharp
{
    [RegisterBlock("web_navigateUrl", Category = "AngleSharp")]
    public class NavigateUrlBlock : IBlock
    {
        public override object Evaluate(Context context)
        {
            Context rootContext = context.GetRootContext();
            string url = Values.Evaluate("url", context).ToString();
            ScrapingBrowser browsingContext = (ScrapingBrowser)rootContext.Variables["__browsingContext"];
            if (rootContext.Variables.ContainsKey("__currentWebPage"))
            {
                rootContext.Variables.Remove("__currentWebPage");
            }

            WebPage newDocument = browsingContext.NavigateToPage(new Uri(url));
            rootContext.Variables.Add("__currentWebPage", newDocument);
            base.Evaluate(context);
            return null;
        }
    }
}
