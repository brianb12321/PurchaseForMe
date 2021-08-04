using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using PurchaseForMe.Core.Code;
using PurchaseForMe.Core.Code.Runner;
using PurchaseForMe.Core.TaskSystem;

namespace PurchaseForMeService.Actors
{
    public abstract class SchedulingBus : ReceiveActor, IWithTimers, IWithUnboundedStash
    {
        public ITimerScheduler Timers { get; set; }
        public IStash Stash { get; set; }
        protected List<RunnerInfo> RunnerInstances { get; }

        protected SchedulingBus()
        {
            RunnerInstances = new List<RunnerInfo>();
            Receive<GetRunnerStatisticsMessage>(message =>
            {
                int numberRunning = RunnerInstances.Count(runner => runner.IsRunning);
                int numberAvailable = RunnerInstances.Count(runner => runner.IsRunning != true);
                Sender.Tell(new GetRunnerStatisticsResponseMessage(numberAvailable, numberAvailable));
            });
            Receive<GetTaskRunnerInfoForAll>(message =>
            {
                Sender.Tell(new TaskRunnerInfoEnumeration(RunnerInstances), Self);
            });
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