using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PurchaseForMe.Core.Project.Nodes.Blockly
{
    public abstract class BlocklyNode : ProjectNode
    {
        public XmlDocument BlocklyWorkspace { get; set; }
    }
}