using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using Akka.Routing;
using IronBlock;
using PurchaseForMe.Core;
using PurchaseForMe.Core.Code.Abstraction;
using PurchaseForMeService.Actors.TaskSystem;
using PurchaseForMeService.Actors.WebPipeline;
using PurchaseForMeService.Blocks;

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
            _pipelineCodeFactory = new BlocklyCodeContext.BlocklyCodeContextFactory();
            IActorRef pipelineBus = _actorSystem.ActorOf(Props
                .Create(() => new PipelineSchedulingBus(_pipelineSchedulingBusLogger, _pipelineCodeFactory)), "pipelineSchedulingBus");

            _taskCodeFactory = new BlocklyCodeContext.BlocklyCodeContextFactory();
            Assembly assembly = Assembly.GetExecutingAssembly();
            foreach (Type type in assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract))
            {
                RegisterBlockAttribute block = type.GetCustomAttribute<RegisterBlockAttribute>();
                if (block != null && (block.Category is "All" or "Selenium"))
                {
                    _taskCodeFactory.AdditionalBlocks.Add(block.BlockName, () => (IBlock)Activator.CreateInstance(type));
                    _pipelineCodeFactory.AdditionalBlocks.Add(block.BlockName, () => (IBlock)Activator.CreateInstance(type));
                }
            }

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
