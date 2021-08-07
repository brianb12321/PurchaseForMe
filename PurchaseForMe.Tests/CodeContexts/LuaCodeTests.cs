using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PurchaseForMe.Core.Code.Abstraction;
using PurchaseForMeService.CodeContexts;

namespace PurchaseForMe.Tests.CodeContexts
{
    public class LuaCodeTests
    {
        [Test]
        public async Task InterpreterProducesRightResult()
        {
            ICodeContextFactory factory = new LuaCodeContext.LuaCodeContextFactory();
            ICodeContext context = factory.Create("return 3 + 3");
            Assert.AreEqual((long)(await context.Execute(null, CancellationToken.None)), 6);
        }

        [Test]
        public async Task LoadMethodWorks()
        {
            ICodeContextFactory factory = new LuaCodeContext.LuaCodeContextFactory();
            ICodeContext lua = factory.Create();
            await lua.Load("return 777");
            long value = (long)await lua.Execute(null, CancellationToken.None);
            Assert.AreEqual(value, 777);
        }

        [Test]
        public async Task AbleToRetreiveVariables()
        {
            LuaCodeContext.LuaCodeContextFactory factory = new LuaCodeContext.LuaCodeContextFactory();
            factory.Variables.Add("favoriteFood", "Chicken Alfredo");
            factory.Variables.Add("number", 3 + 3);

            ICodeContext lua = factory.Create();
            await lua.Load("return favoriteFood");
            var value = await lua.Execute(null, CancellationToken.None);
            Assert.AreEqual(value.ToString(), "Chicken Alfredo");
            await lua.Load("return number + 1");
            value = await lua.Execute(null, CancellationToken.None);
            Assert.AreEqual((long)value, 7);
        }

        [Test]
        public async Task CanCallAddedFunctions()
        {
            LuaCodeContext.LuaCodeContextFactory factory = new LuaCodeContext.LuaCodeContextFactory();
            factory.Functions.Add("runMe", new Func<string>(() => "Sir, you have called me."));

            ICodeContext lua = factory.Create("return runMe()");
            string result = (await lua.Execute(null, CancellationToken.None)).ToString();
            Assert.AreEqual(result, "Sir, you have called me.");
        }

        [Test]
        public void ExecuteReturnsExceptionInTask()
        {
            ICodeContextFactory factory = new LuaCodeContext.LuaCodeContextFactory();
            ICodeContext context = factory.Create("Cats are cool; Turtles are cool; Dolphins are cool");

            Task result = context.Execute(null, CancellationToken.None);
            Assert.IsNotNull(result.Exception);
        }
    }
}