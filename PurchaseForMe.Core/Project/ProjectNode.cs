using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseForMe.Core.Project
{
    public abstract class ProjectNode
    {
        public Guid NodeGuid { get; set; }
        public string NodeType { get; protected set; }
    }
}