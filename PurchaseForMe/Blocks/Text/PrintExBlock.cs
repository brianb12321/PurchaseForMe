using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IronBlock;
using IronBlock.Blocks;

namespace PurchaseForMe.Blocks.Text
{
    public class PrintExBlock : IBlock
    {
        public override object Evaluate(Context context)
        {
            var text = Values.Evaluate("TEXT", context);
            CodeChannelWriter channelWriter = (CodeChannelWriter)context.GetRootContext().Variables["__standardOut"];
            channelWriter.WriteLine(text);
            base.Evaluate(context);
            return null;
        }
    }
}