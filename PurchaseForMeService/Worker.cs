using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using Akka.Routing;
using PurchaseForMe.Core;
using PurchaseForMeService.Actors.TaskSystem;
using PurchaseForMeService.Actors.WebPipeline;

namespace PurchaseForMeService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ILogger<PipelineSchedulingBus> _pipelineSchedulingBusLogger;
        private readonly ILogger<TaskSchedulingBus> _taskSchedulingBusLogger;
        private ActorSystem _actorSystem;
        public Worker(ILogger<Worker> logger, ILogger<PipelineSchedulingBus> pipelineSchedulingBusLogger, ILogger<TaskSchedulingBus> taskSchedulingBusLogger)
        {
            _logger = logger;
            _pipelineSchedulingBusLogger = pipelineSchedulingBusLogger;
            _taskSchedulingBusLogger = taskSchedulingBusLogger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _actorSystem = ActorSystem.Create("purchaseForMe", ConfigurationFactory.ParseString(File.ReadAllText("akkaconf.txt")));
            IActorRef pipelineBus = _actorSystem.ActorOf(Props
                .Create(() => new PipelineSchedulingBus(_pipelineSchedulingBusLogger)), "pipelineSchedulingBus");

            IActorRef taskBus = _actorSystem.ActorOf(Props
                    .Create(() => new TaskSchedulingBus(_taskSchedulingBusLogger, pipelineBus)), "taskSchedulingBus");

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _actorSystem.Terminate();
            return Task.CompletedTask;
        }
    }
}
