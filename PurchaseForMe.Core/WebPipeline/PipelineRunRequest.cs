using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseForMe.Core.WebPipeline
{
    public class PipelineRunRequest
    {
        public string WorkspaceXml { get; set; }
        public bool ReturnCode { get; set; }
        public Guid SessionId { get; }

        public PipelineRunRequest()
        {
            SessionId = Guid.NewGuid();
        }
    }
}