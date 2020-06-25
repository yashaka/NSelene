using NUnit.Framework;
using static NSelene.Selene;

namespace NSelene.Tests.Integration.SharedDriver.SeleneElementSpec
{
    using Harness;
    using Microsoft.VisualBasic.CompilerServices;

    [TestFixture]
    public class SeleneElement_ExecuteScript_Should : BaseTest
    {

        [TearDown]
        public void TeardownTest()
        {
            Configuration.Timeout = 4;
        }
        
        [Test]
        public void EchoShouldReturnArg()
        {
            Given.OpenedPageWithBody("<div/>");
            var ele = S("div");

            var result = ele.GetFromExecuteScript("return args[0]", 25L);

            Assert.AreEqual(25L, result);
        }
    }
}

