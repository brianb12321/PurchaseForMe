using System;

namespace PurchaseForMe.Core.WebPipeline
{
    /// <summary>
    /// Represents a model returned from a pipeline.
    /// </summary>
    [Serializable]
    public class WebDataModel
    {
        public dynamic ModelData { get; set; }
    }
}