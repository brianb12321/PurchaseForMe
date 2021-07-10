using System;
using AngleSharp.Dom;
using HtmlAgilityPack;
using IronBlock;
using IronBlock.Blocks;

namespace PurchaseForMeService.Blocks.Web.AngleSharp
{
    [RegisterBlock("web_clickElement", Category = "AngleSharp")]
    public class ClickElementBlock : IBlock
    {
        public override object Evaluate(Context context)
        {
            throw new NotImplementedException();
        }
    }
}