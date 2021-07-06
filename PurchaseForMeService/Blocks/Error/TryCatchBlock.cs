using System;
using IronBlock;
using IronBlock.Blocks;

namespace PurchaseForMeService.Blocks.Error
{
    [RegisterBlock("error_tryCatch")]
    public class TryCatchBlock : IBlock
    {
        public override object Evaluate(Context context)
        {
            Statement tryStatement = Statements.Get("tryStatement");
            Statement catchStatement = Statements.Get("catchStatement");
            try
            {
                tryStatement.Evaluate(context);
            }
            catch (Exception e)
            {
                catchStatement.Evaluate(context);
            }

            return null;
        }
    }
}