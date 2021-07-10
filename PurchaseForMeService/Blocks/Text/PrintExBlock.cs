using System;
using IronBlock;
using IronBlock.Blocks;
using PurchaseForMe;

namespace PurchaseForMeService.Blocks.Text
{
    public class PrintExBlock : IBlock
    {
        public override object Evaluate(Context context)
        {
            var text = Values.Evaluate("TEXT", context);
            CodeChannelWriter channelWriter = (CodeChannelWriter)context.GetRootContext().Variables["__standardOut"];
            channelWriter.WriteLine(text);
            Console.WriteLine(text);
            base.Evaluate(context);
            return null;
        }
    }
}