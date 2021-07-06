using System;
using IronBlock;
using IronBlock.Blocks;

namespace PurchaseForMeService.Blocks.Error
{
    [RegisterBlock("error_throw")]
    public class ThrowBlock : IBlock
    {
        public override object Evaluate(Context context)
        {
            Exception exception = (Exception)this.Values.Evaluate("errorObject", context);
            throw exception;
        }
    }
}
