using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using PurchaseForMe.Core.Code.Abstraction;
using PurchaseForMe.Core.Code.Instance;

namespace PurchaseForMeService.Actors
{
    public class CodeInstanceActor : ReceiveActor
    {
        protected virtual Task ConfigureCodeContext(ICodeContext context, InstanceStartMessage message)
        {
            return Task.CompletedTask;
        }
        public CodeInstanceActor(ICodeContextFactory factory)
        {
            ReceiveAsync<InstanceStartMessage>(async message =>
            {
                ICodeContext context = factory.Create(message.Code);
                await ConfigureCodeContext(context, message);
                try
                {
                    var result = await context.Execute(null, CancellationToken.None);
                    Sender.Tell(new InstanceFinishedMessage(true, result), Self);
                }
                catch (Exception e)
                {
                    Sender.Tell(new InstanceFinishedMessage(false, e));
                }
            });
        }
    }
}