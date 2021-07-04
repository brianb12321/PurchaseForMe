using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IronBlock;
using IronBlock.Blocks;

namespace PurchaseForMe.Blocks.Timer
{
    [RegisterBlock("timer_loopControl")]
    public class LoopControlBlock : IBlock
    {
        public enum LoopControlAction
        {
            Break
        }
        public override object Evaluate(Context context)
        {
            LoopControlAction action = (LoopControlAction)Enum.Parse(typeof(LoopControlAction), Fields.Get("loopControlAction"));
            switch (action)
            {
                case LoopControlAction.Break:
                    context.EscapeMode = EscapeMode.Break;
                    break;
            }

            return null;
        }
    }
}