using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using IronBlock;
using PurchaseForMe.Core.Code.Abstraction;
using PurchaseForMeService.Blocks;

namespace PurchaseForMeService.CodeContexts
{
    public class BlocklyCodeContext : ICodeContext
    {
        public class BlocklyCodeContextFactory : ICodeContextFactory
        {
            public Dictionary<string, Func<IBlock>> AdditionalBlocks { get; }

            public BlocklyCodeContextFactory(string blockType)
            {
                AdditionalBlocks = new Dictionary<string, Func<IBlock>>();
                Assembly assembly = Assembly.GetExecutingAssembly();
                foreach (Type type in assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract))
                {
                    RegisterBlockAttribute block = type.GetCustomAttribute<RegisterBlockAttribute>();
                    if (block != null && (block.Category == "All" || block.Category == blockType))
                    {
                        AdditionalBlocks.Add(block.BlockName, () => (IBlock)Activator.CreateInstance(type));
                    }
                }
            }

            public ICodeContext Create()
            {
                throw new NotImplementedException();
            }

            public ICodeContext Create(string code)
            {
                XmlDocument document = new XmlDocument();
                document.LoadXml(code);
                return new BlocklyCodeContext(document, AdditionalBlocks);
            }
        }
        public Workspace Workspace { get; }
        private readonly XmlDocument _document;
        public Dictionary<string, object> Variables { get; }
        public Dictionary<string, Delegate> Functions { get; }

        private BlocklyCodeContext(XmlDocument workspace, IReadOnlyDictionary<string, Func<IBlock>> additionalBlocks)
        {
            _document = workspace;
            Parser parser = new Parser();
            parser.AddStandardBlocksEx();
            foreach (var block in additionalBlocks)
            {
                parser.AddBlock(block.Key, block.Value);
            }
            Workspace = parser.Parse(workspace.InnerXml, true);
            Variables = new Dictionary<string, object>();
            Functions = new Dictionary<string, Delegate>();
        }
        public override string ToString()
        {
            return _document.InnerXml;
        }


        public Task<object> Execute(string[] args, CancellationToken token)
        {
            Context rootContext = new Context();
            foreach (var variable in Variables)
            {
                rootContext.Variables.Add(variable.Key, variable.Value);
            }
            foreach (var function in Functions)
            {
                rootContext.Functions.Add(function.Key, function.Value);
            }

            try
            {
                return Task.FromResult(Workspace.Evaluate(rootContext));
            }
            catch (Exception e)
            {
                return Task.FromException<object>(e);
            }
        }
    }
}