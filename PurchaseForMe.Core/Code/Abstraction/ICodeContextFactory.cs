using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseForMe.Core.Code.Abstraction
{
    public interface ICodeContextFactory
    {
        ICodeContext Create();
        ICodeContext Create(string code);
    }
}