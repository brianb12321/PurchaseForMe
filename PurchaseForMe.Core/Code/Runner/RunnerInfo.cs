using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;

namespace PurchaseForMe.Core.Code.Runner
{
    public class RunnerInfo
    {
        public IActorRef Actor { get; }
        public IActorRef CallingActor { get; set; }
        public bool IsRunning { get; set; }
        public int RunnerId { get; }

        public RunnerInfo(IActorRef actor, int runnerId)
        {
            Actor = actor;
            RunnerId = runnerId;
        }
    }
}