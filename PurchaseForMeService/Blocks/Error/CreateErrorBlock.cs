using System;
using IronBlock;
using IronBlock.Blocks;

namespace PurchaseForMeService.Blocks.Error
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
