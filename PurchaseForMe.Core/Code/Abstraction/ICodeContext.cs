using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PurchaseForMe.Core.Code.Abstraction
{
    /// <summary>
    /// Represents the currently executing context.
    /// </summary>
    public interface ICodeContext
    {
        Dictionary<string, object> Variables { get; }
        Dictionary<string, Delegate> Functions { get; }
        Task Load(string code);
        Task<object> Execute(string[] args, CancellationToken token);
    }
}