using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseForMeAdminConsole
{
    public class ConsoleModeDescriptor
    {
        public string Description { get; }
        private readonly Func<ConsoleMode> _modeFactory;

        public ConsoleModeDescriptor(string description, Func<ConsoleMode> factory)
        {
            Description = description;
            _modeFactory = factory;
        }

        public ConsoleMode GetConsoleMode()
        {
            return _modeFactory();
        }
    }
}