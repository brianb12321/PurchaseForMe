
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseForMe.Core.WebPipeline
{
    public interface IWebPipelineLifetime
    {
        Action<PipelineInstanceResult> ResultIn { get; set; }
    }
}