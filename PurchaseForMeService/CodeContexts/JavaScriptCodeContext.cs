using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jint;
using PurchaseForMe.Core.Code.Abstraction;

namespace PurchaseForMeService.CodeContexts
{
    public class JavaScriptCodeContext : ICodeContext
    {
        public class JavaScriptCodeContextFactory : ICodeContextFactory
        {
            public ICodeContext Create()
            {
                return Create(string.Empty);
            }

            public ICodeContext Create(string code)
            {
                var context = new JavaScriptCodeContext(code);
                return context;
            }
        }

        public Dictionary<string, object> Variables { get; }
        public Dictionary<string, Delegate> Functions { get; }
        private Engine _engine;
        private string _code;

        private JavaScriptCodeContext(string code)
        {
            Variables = new Dictionary<string, object>();
            Functions = new Dictionary<string, Delegate>();
            _code = code;
        }

        public Task<object> Execute(string[] args, CancellationToken token)
        {
            _engine = new Engine();
            _engine.SetValue("args", args);
            _engine.Execute(_code);
            return Task.FromResult(_engine.GetValue("exitCode").ToObject());
        }
    }
}