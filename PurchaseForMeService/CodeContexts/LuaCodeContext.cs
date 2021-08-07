using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLua;
using NLua.Exceptions;
using PurchaseForMe.Core.Code.Abstraction;

namespace PurchaseForMeService.CodeContexts
{
    public class LuaCodeContext : ICodeContext
    {
        public class LuaCodeContextFactory : ICodeContextFactory
        {
            public Dictionary<string, object> Variables { get; }
            public Dictionary<string, Delegate> Functions { get; }

            public LuaCodeContextFactory()
            {
                Variables = new Dictionary<string, object>();
                Functions = new Dictionary<string, Delegate>();
            }

            public ICodeContext Create()
            {
                return Create(string.Empty);
            }

            public ICodeContext Create(string code)
            {
                ICodeContext context = new LuaCodeContext(Variables, Functions);
                context.Load(code);
                return context;
            }
        }

        public Dictionary<string, object> Variables { get; }
        public Dictionary<string, Delegate> Functions { get; }
        private readonly Lua _state;
        private string _code;
        private LuaCodeContext(Dictionary<string, object> variables = null, Dictionary<string, Delegate> functions = null)
        {
            _state = new Lua();
            Variables = variables ?? new Dictionary<string, object>();
            Functions = functions ?? new Dictionary<string, Delegate>();
            foreach (var variable in Variables)
            {
                _state[variable.Key] = variable.Value;
            }
            foreach (var function in Functions)
            {
                _state[function.Key] = function.Value;
            }
        }
        public Task Load(string code)
        {
            _code = code;
            return Task.CompletedTask;
        }

        public Task<object> Execute(string[] args, CancellationToken token)
        {
            _state["args"] = args;
            try
            {
                var result = _state.DoString(_code);
                return Task.FromResult(result[0]);
            }
            catch (LuaException e)
            {
                return Task.FromException<object>(e);
            }
        }
    }
}