using System;

namespace PurchaseForMeService.Blocks
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