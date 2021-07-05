using AngleSharp;
using AngleSharp.Dom;
using IronBlock;
using IronBlock.Blocks;
using OpenQA.Selenium;

namespace PurchaseForMe.Blocks.Web.AngleSharp
{
    [RegisterBlock("web_navigateUrl", Category = "AngleSharp")]
    public class NavigateUrlBlock : IBlock
    {
        public override object Evaluate(Context context)
        {
            Context rootContext = context.GetRootContext();
            string url = Values.Evaluate("url", context).ToString();
            IBrowsingContext browsingContext = (IBrowsingContext)rootContext.Variables["__browsingContext"];
            if (rootContext.Variables.ContainsKey("__currentWebPage"))
            {
                IDocument document = (IDocument)rootContext.Variables["__currentWebPage"];
                document.Dispose();
                rootContext.Variables.Remove("__currentWebPage");
            }
            IDocument newDocument = browsingContext.OpenAsync(url).GetAwaiter().GetResult();
            rootContext.Variables.Add("__currentWebPage", newDocument);
            base.Evaluate(context);
            return null;
        }
    }
}
