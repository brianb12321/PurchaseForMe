using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Js;
using IronBlock;
using IronBlock.Blocks;
using ScrapySharp.Network;

namespace PurchaseForMeService.Blocks.Web.AngleSharp
{
    [RegisterBlock("web_executeJavascript", Category = "AngleSharp")]
    public class ExecuteJavascriptBlock : IBlock
    {
        public override object Evaluate(Context context)
        {
            throw new NotImplementedException();
        }
    }
}
