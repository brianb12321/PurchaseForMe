using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster;

namespace PurchaseForMeAdminConsole
{
    public abstract class ActorSpecificMode : ConsoleMode
    {
        protected ActorSystem System { get; }
        protected Member Member { get; }
        protected ActorSelection SelectedActor { get; set; }
        protected string DefaultActorPath { get; }
        public string ModeName { get; }

        protected ActorSpecificMode(ActorSystem system, Member member, string modeName, string defaultActorPath)
        {
            System = system;
            Member = member;
            ModeName = modeName;
            DefaultActorPath = defaultActorPath;
        }

        protected abstract Task HandleOtherCommands(string[] tokens);
        public override async Task HandleCommand(string[] token)
        {
            if (token[0] == "actorSelect")
            {
                string actorPath = token.Length >= 2 ? token[1] : DefaultActorPath;
                //If path has / remove it.
                string normalizedString = $"{Member.Address}/{actorPath.TrimStart('/')}";
                ActorSelection selection = System.ActorSelection(normalizedString);
                //Attempt to resolve actor.
                try
                {
                    await selection.ResolveOne(TimeSpan.FromMinutes(1));
                    SelectedActor = selection;
                }
                catch (ActorNotFoundException)
                {
                    Console.WriteLine($"Actor \"{normalizedString}\" cannot be contacted.");
                }
            }
            else if (token[0] == "exit")
            {
                ExitFlag = true;
            }
            else
            {
                await HandleOtherCommands(token);
            }
        }

        public override void PrintPrompt()
        {
            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{Member.Address}");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write($"[{ModeName}");
            if (SelectedActor != null)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($" ({SelectedActor.PathString})");
            }
            Console.Write("]");
            Console.ForegroundColor = defaultColor;
            Console.Write(">");
        }
    }
}