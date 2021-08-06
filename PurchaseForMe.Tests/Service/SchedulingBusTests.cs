using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using PurchaseForMe.Core.Code;
using PurchaseForMe.Core.Code.Abstraction;
using PurchaseForMeService;
using PurchaseForMeService.Actors.TaskSystem;
using PurchaseForMeService.Actors.WebPipeline;

namespace PurchaseForMe.Tests.Service
{
    public class SchedulingBusTests
    {
        private ActorSystem _system;
        private IActorRef _taskSchedulingBus;
        private IActorRef _pipelineBus;
        
        [OneTimeSetUp]
        public void Setup()
        {
            _system = ActorSystem.Create("purchaseForMe-tests");
            _pipelineBus =
                _system.ActorOf(Props.Create(() => new PipelineSchedulingBus(new NullLogger<PipelineSchedulingBus>(), new BlocklyCodeContext.BlocklyCodeContextFactory())));
            _taskSchedulingBus = _system.ActorOf(Props.Create(() =>
                new TaskSchedulingBus(new NullLogger<TaskSchedulingBus>(), _pipelineBus, new BlocklyCodeContext.BlocklyCodeContextFactory())));
        }

        [Test]
        public async Task TaskSchedulingBus_RunnerInstancesPopulated()
        {
            GetRunnerStatisticsResponseMessage message =
                await _taskSchedulingBus.Ask<GetRunnerStatisticsResponseMessage>(new GetRunnerStatisticsMessage());

            Assert.AreEqual(message.NumberOfRunnersAvailable, 5);
        }

        public async Task TaskSchedulingBus_RunnerTransitionedIntoRunning()
        {
            
        }
        [Test]
        public async Task PipelineSchedulingBus_RunnerInstancesPopulated()
        {
            GetRunnerStatisticsResponseMessage message =
                await _pipelineBus.Ask<GetRunnerStatisticsResponseMessage>(new GetRunnerStatisticsMessage());

            Assert.AreEqual(message.NumberOfRunnersAvailable, 5);
        }
    }
}