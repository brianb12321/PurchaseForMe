using System;
using System.Threading;
using IronBlock;
using IronBlock.Blocks;

namespace PurchaseForMeService.Blocks.Timer
{
    [RegisterBlock("timer_loopWait")]
    public class LoopWaitBlock : IBlock
    {
        public enum TimeUnit
        {
            Milliseconds, Seconds, Minutes, Hours
        }
        public override object Evaluate(Context context)
        {
            TimeUnit timeUnit = (TimeUnit) Enum.Parse(typeof(TimeUnit), Fields.Get("waitDurationType"));
            double time = (double)Values.Evaluate("waitTime", context);
            IronBlock.Statement body = Statements.Get("loopBody");
            while (context.EscapeMode != EscapeMode.Break)
            {
                body.Evaluate(context);
                TimeSpan timeSpan = TimeSpan.Zero;;
                switch (timeUnit)
                {
                    case TimeUnit.Milliseconds:
                        timeSpan = TimeSpan.FromMilliseconds(time);
                        break;
                    case TimeUnit.Seconds:
                        timeSpan = TimeSpan.FromSeconds(time);
                        break;
                    case TimeUnit.Minutes:
                        timeSpan = TimeSpan.FromMinutes(time);
                        break;
                    case TimeUnit.Hours:
                        timeSpan = TimeSpan.FromHours(time);
                        break;
                    default:
                        throw new ArgumentException($"{timeUnit} has not been handled.");
                }
                Thread.Sleep(timeSpan);
            }

            base.Evaluate(context);
            return null;
        }
    }
}