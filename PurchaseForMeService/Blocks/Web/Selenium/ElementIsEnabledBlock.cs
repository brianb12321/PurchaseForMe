using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom;
using IronBlock;
using IronBlock.Blocks;

namespace PurchaseForMeService.Blocks.Web.Selenium
{
    [RegisterBlock("web_elementIsEnabled", Category = "AngleSharp")]
    public class ElementIsEnabledBlock : IBlock
    {
        public override object Evaluate(Context context)
        {
            IElement element = (IElement) Values.Evaluate("element", context);
            return (element.HasAttribute("disabled") != true);
        }
    }
}