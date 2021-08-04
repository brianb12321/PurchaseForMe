using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console = System.Console;

namespace PurchaseForMeAdminConsole
{
    public abstract class ConsoleMode
    {
        protected bool ExitFlag { get; set; } = false;
        public abstract void PrintPrompt();
        public virtual async Task Start()
        {
            while (!ExitFlag)
            {
                PrintPrompt();
                string command = Console.ReadLine();
                string[] tokens = command.Split(' ');
                await HandleCommand(tokens);
            }
        }
        public abstract Task HandleCommand(string[] token);
    }
}