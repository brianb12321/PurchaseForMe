using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PurchaseForMe.Core.Project.Nodes.Blockly;

namespace PurchaseForMe.Core.WebPipeline
{
    public class PipelineRunRequest
    {
        public BlocklyPipelineNode PipelineNode { get; set; }
        public bool ReturnCode { get; set; }
    }
}