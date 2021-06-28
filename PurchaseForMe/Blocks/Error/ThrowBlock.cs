using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IronBlock;
using IronBlock.Blocks;

namespace PurchaseForMe.Blocks.Error
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
