using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using IronBlock;
using PurchaseForMe.Core;
using PurchaseForMeService.Actors.TaskSystem;
using PurchaseForMeService.Actors.WebPipeline;
using PurchaseForMeService.Blocks;
using PurchaseForMeService.CodeContexts;

namespace PurchaseForMeService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ILogger<PipelineSchedulingBus> _pipelineSchedulingBusLogger;
        private readonly ILogger<TaskSchedulingBus> _taskSchedulingBusLogger;
        private BlocklyCodeContext.BlocklyCodeContextFactory _taskCodeFactory;
        private BlocklyCodeContext.BlocklyCodeContextFactory _pipelineCodeFactory;
        private ActorSystem _actorSystem;
        public Worker(ILogger<Worker> logger, ILogger<PipelineSchedulingBus> pipelineSchedulingBusLogger, ILogger<TaskSchedulingBus> taskSchedulingBusLogger)
        {
            _logger = logger;
            _pipelineSchedulingBusLogger = pipelineSchedulingBusLogger;
            _taskSchedulingBusLogger = taskSchedulingBusLogger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Config configuration = ConfigurationFactory.ParseString(File.ReadAllText("akkaconf-service.txt")).
                CreateConfigurationWithEnvironment();

            _actorSystem = ActorSystem.Create("purchaseForMe", configuration);
            _pipelineCodeFactory = new BlocklyCodeContext.BlocklyCodeContextFactory("Selenium");
            IActorRef pipelineBus = _actorSystem.ActorOf(Props
                .Create(() => new PipelineSchedulingBus(_pipelineSchedulingBusLogger, _pipelineCodeFactory)), "pipelineSchedulingBus");

            _taskCodeFactory = new BlocklyCodeContext.BlocklyCodeContextFactory("Selenium");

            IActorRef taskBus = _actorSystem.ActorOf(Props
                    .Create(() => new TaskSchedulingBus(_taskSchedulingBusLogger, pipelineBus, _taskCodeFactory)), "taskSchedulingBus");

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _actorSystem.Terminate();
            return Task.CompletedTask;
        }
    }
}