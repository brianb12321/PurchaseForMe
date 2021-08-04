using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster;
using PurchaseForMeAdminConsole.Modes.Actors;

namespace PurchaseForMeAdminConsole.Modes
{
    public class NodeSelectionMode : ConsoleMode
    {
        private readonly Member _member;
        private Dictionary<string, ConsoleModeDescriptor> _availableModes;

        public NodeSelectionMode(Member member, ActorSystem system)
        {
            _member = member;
            _availableModes = new Dictionary<string, ConsoleModeDescriptor>();
            _availableModes.Add("taskSchedulingBus", new ConsoleModeDescriptor("Manage running task runners.", () => new TaskSchedulingBusMode(system, member)));
        }
        public override void PrintPrompt()
        {
            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{_member.Address}");
            Console.ForegroundColor = defaultColor;
            Console.Write(">");
        }

        public override async Task HandleCommand(string[] token)
        {
            if (token[0] == "details")
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Address: {_member.Address}");
                sb.Append($"Roles: ");
                foreach (string role in _member.Roles)
                {
                    sb.Append(role);
                    sb.Append(", ");
                }

                sb.Length -= 2;
                sb.AppendLine();
                sb.AppendLine($"Status: {_member.Status}");
                Console.WriteLine(sb.ToString());
            }
            else if (token[0] == "console")
            {
                if (token.Length < 2)
                {
                    foreach (KeyValuePair<string, ConsoleModeDescriptor> mode in _availableModes)
                    {
                        Console.WriteLine($"{mode.Key}: {mode.Value.Description}");
                    }

                    return;
                }
                try
                {
                    ConsoleModeDescriptor selectedModeDescriptor = _availableModes[token[1]];
                    ConsoleMode selectedMode = selectedModeDescriptor.GetConsoleMode();
                    await selectedMode.Start();
                }
                catch (KeyNotFoundException)
                {
                    Console.WriteLine($"No console found for \"{token[1]}\"");
                }

            }
            else if (token[0] == "exit")
            {
                ExitFlag = true;
            }
        }
    }
}