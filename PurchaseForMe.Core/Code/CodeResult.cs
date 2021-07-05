using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseForMe.Core.Code
{
    public class CodeResult
    {
        public Guid CodeGuid { get; set; }
        public bool IsSuccessful { get; set; }
        public string ResultMessage { get; set; }
    }
}