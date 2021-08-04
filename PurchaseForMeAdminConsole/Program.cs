using System;
using System.IO;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster;
using Akka.Configuration;
using PurchaseForMe.Core;
using PurchaseForMeAdminConsole.Modes;

namespace PurchaseForMeAdminConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string clusterAddress = string.Empty;
            Config configuration = ConfigurationFactory.ParseString(File.ReadAllText("akkaconf-adminConsole.txt"))
                .CreateConfigurationWithEnvironment();

            ActorSystem system = ActorSystem.Create("purchaseForMe", configuration);
            Cluster cluster = Cluster.Get(system);
            Console.WriteLine("Admin console connected to cluster.");
            Console.WriteLine();
            ConsoleMode console = new GlobalMode(system, cluster);
            await console.Start();
            await system.Terminate();
        }
    }
}