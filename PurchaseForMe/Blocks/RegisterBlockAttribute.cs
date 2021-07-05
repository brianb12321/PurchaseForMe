using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PurchaseForMe.Blocks
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class RegisterBlockAttribute : Attribute
    {
        public string BlockName { get; }
        public string Category { get; set; } = "All";

        public RegisterBlockAttribute(string blockName)
        {
            BlockName = blockName;
        }
    }
}