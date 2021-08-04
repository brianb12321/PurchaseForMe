using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster;
using PurchaseForMe.Core.Code.Runner;
using PurchaseForMe.Core.TaskSystem;

namespace PurchaseForMeAdminConsole.Modes.Actors
{
    public class TaskSchedulingBusMode : ActorSpecificMode
    {
        public TaskSchedulingBusMode(ActorSystem system, Member member) : base(system, member, "taskSchedulingBus", "/user/taskSchedulingBus")
        {
        }

        protected override async Task HandleOtherCommands(string[] tokens)
        {
            if (tokens[0] == "task")
            {
                IActorRef actor = await SelectedActor.ResolveOne(TimeSpan.FromMinutes(1));
                TaskRunnerInfoEnumeration runners = await actor.Ask<TaskRunnerInfoEnumeration>(new GetTaskRunnerInfoForAll());
                foreach (RunnerInfo info in runners.Runners)
                {
                    Console.WriteLine($"{info.RunnerId} ({(info.IsRunning ? "Running" : "Not Running")}): {info.Actor}");
                }
            }
        }
    }
}