using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using PurchaseForMe.Actors.TaskSystem;

namespace PurchaseForMe.Actors
{
    public abstract class SchedulingBus : ReceiveActor, IWithTimers, IWithUnboundedStash
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

        public ITimerScheduler Timers { get; set; }
        public IStash Stash { get; set; }
        protected List<RunnerInfo> RunnerInstances { get; }

        protected SchedulingBus()
        {
            RunnerInstances = new List<RunnerInfo>();
        }

        protected RunnerInfo SendMessageToOpenRunner(object message, IActorRef callingActor)
        {
            //Check if a task runner is available.
            int runnersAvailable = RunnerInstances.Count(r => !r.IsRunning);
            if (runnersAvailable == 0)
            {
                return null;
            }

            var taskRunner = RunnerInstances.First(r => !r.IsRunning);
            taskRunner.CallingActor = callingActor;
            taskRunner.Actor.Tell(message, Self);
            return taskRunner;
        }
    }
}