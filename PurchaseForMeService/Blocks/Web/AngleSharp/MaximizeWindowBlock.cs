using System;
using IronBlock;

namespace PurchaseForMeService.Blocks.Web.AngleSharp
{
    [RegisterBlock("web_maximizeWindow", Category = "AngleSharp")]
    public class MaximizeWindowBlock : IBlock
    {
        public override object Evaluate(Context context)
        {
            throw new PlatformNotSupportedException("AngleSharp does not have window.");
        }
    }
}