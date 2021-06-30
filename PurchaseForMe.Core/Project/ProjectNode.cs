using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PurchaseForMe.Core.Project
{
    public abstract class ProjectNode
    {
        public Guid NodeGuid { get; protected set; }
        public NodeType NodeType { get; protected set; }
        public string NodeName { get; set; }

        protected ProjectNode()
        {
            NodeGuid = Guid.NewGuid();
        }
    }
}