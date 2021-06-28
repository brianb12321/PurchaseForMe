using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IronBlock;
using IronBlock.Blocks;

namespace PurchaseForMe.Blocks.Error
{
    [RegisterBlock("error_createError")]
    public class CreateErrorBlock : IBlock
    {
        public override object Evaluate(Context context)
        {
            string errorMessage = Values.Evaluate("errorMessage", context).ToString();
            return new Exception(errorMessage);
        }
    }
}
