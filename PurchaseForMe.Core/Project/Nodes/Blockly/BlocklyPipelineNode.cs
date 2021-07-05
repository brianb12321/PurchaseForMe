using System;
using System.Xml;

namespace PurchaseForMe.Core.Project.Nodes.Blockly
{
    [Serializable]
    public class BlocklyPipelineNode : BlocklyNode
    {
        public string CodeName { get; set; }
        public BlocklyPipelineNode()
        {
            NodeType = NodeType.BlocklyPipeline;
        }
    }
}
