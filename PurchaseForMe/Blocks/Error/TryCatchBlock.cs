using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IronBlock;
using IronBlock.Blocks;

namespace PurchaseForMe.Blocks.Error
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