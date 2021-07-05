using System;
using PurchaseForMe.Core.Code;

namespace PurchaseForMe.Core.WebPipeline
{
    public class PipelineInstanceResult : CodeResult
    {
        public WebDataModel WebDataModel { get; set; }
        public string CompiledCode { get; set; }
    }
}