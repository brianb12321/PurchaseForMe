using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using IronBlock;
using IronBlock.Blocks;
using PurchaseForMe.Core.Code.Abstraction;

namespace PurchaseForMeService
{
    public class BlocklyCodeContext : ICodeContext
    {
        public class BlocklyCodeContextFactory : ICodeContextFactory
        {
            public Dictionary<string, Func<IBlock>> AdditionalBlocks { get; }

            public BlocklyCodeContextFactory()
            {
                AdditionalBlocks = new Dictionary<string, Func<IBlock>>();
            }
            public ICodeContext Create(string code)
            {
                XmlDocument document = new XmlDocument();
                document.LoadXml(code);
                return new BlocklyCodeContext(document, AdditionalBlocks);
            }
        }
        public Workspace Workspace { get; }
        private XmlDocument _document;
        public Dictionary<string, object> Variables { get; }
        public Dictionary<string, object> Functions { get; }

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
            Functions = new Dictionary<string, object>();
        }
        public override string ToString()
        {
            return _document.InnerXml;
        }

        public Task Load(string code)
        {
            throw new NotImplementedException();
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
                rootContext.Functions.Add(function);
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