using Akka.Actor;

namespace PurchaseForMe.Core
{
    public delegate IActorRef CodeMonitorSignalRFactory();
    public delegate IActorRef PipelineSchedulingBusFactory();
    public delegate IActorRef UserManagerActorFactory();
    public delegate IActorRef TaskSchedulingBusFactory();
    public delegate IActorRef ProjectManagerFactory();
}