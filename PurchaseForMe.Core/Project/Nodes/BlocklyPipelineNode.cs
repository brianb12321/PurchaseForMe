using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PurchaseForMe.Core.Project.Nodes
{
    [Serializable]
    public class BlocklyPipelineNode : ProjectNode
    {
        public XmlDocument BlocklyWorkspace { get; set; }
        public BlocklyPipelineNode()
        {
            NodeType = NodeType.BlocklyPipeline;
            BlocklyWorkspace = new XmlDocument();
        }
    }
}
