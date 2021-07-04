using System;

namespace PurchaseForMe.Core.WebPipeline
{
    public class PipelineInstanceResult
    {
        public WebDataModel WebDataModel { get; set; }
        public bool IsSuccessful { get; set; }
        public string CompiledCode { get; set; }
        public Guid SessionId { get; set; }
    }
}