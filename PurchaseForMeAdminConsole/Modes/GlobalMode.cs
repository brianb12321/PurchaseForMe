using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster;

namespace PurchaseForMeAdminConsole.Modes
{
    public class GlobalMode : ConsoleMode
    {
        private readonly ActorSystem _actorSystem;
        private readonly Cluster _cluster;
        public GlobalMode(ActorSystem actorSystem, Cluster cluster)
        {
            _actorSystem = actorSystem;
            _cluster = cluster;
        }

        public override void PrintPrompt()
        {
            Console.Write(">");
        }

        public override async Task HandleCommand(string[] tokens)
        {
            if (tokens[0] == "showNodes")
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < _cluster.State.Members.Count; i++)
                {
                    sb.Append($"{i}: {_cluster.State.Members[i].Address} ");
                    sb.Append('[');
                    foreach (string role in _cluster.State.Members[i].Roles)
                    {
                        sb.Append(role);
                        sb.Append(", ");
                    }

                    sb.Length -= 2;
                    sb.Append(']');
                    sb.AppendLine();
                }
                Console.WriteLine(sb.ToString());
            }
            else if (tokens[0] == "select")
            {
                if (tokens.Length < 2)
                {
                    Console.WriteLine("You must specify a server-node to connect");
                    return;
                }

                if (int.TryParse(tokens[1], out int nodeNumber))
                {
                    if (nodeNumber >= _cluster.State.Members.Count)
                    {
                        Console.WriteLine("Node number must be in range.");
                        return;
                    }
                    Console.WriteLine($"Connecting to node with number \"{nodeNumber}\"");
                    NodeSelectionMode nodeSelectionMode =
                        new NodeSelectionMode(_cluster.State.Members[nodeNumber], _actorSystem);
                    await nodeSelectionMode.Start();
                }
                else
                {
                    Console.WriteLine("Invalid node-number.");
                }
            }
            else if (tokens[0] == "exit")
            {
                ExitFlag = true;
            }
        }
    }
}