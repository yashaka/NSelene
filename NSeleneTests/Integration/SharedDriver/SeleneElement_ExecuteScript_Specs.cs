using NUnit.Framework;
using static NSelene.Selene;

namespace NSelene.Tests.Integration.SharedDriver.SeleneElementSpec
{
    using Harness;

    [TestFixture]
    public class SeleneElement_ExecuteScript_Specs : BaseTest
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
            var element = S("div");

            var result = element.ExecuteScript("return args[0]", 25L);

            Assert.AreEqual(25L, result);
        }
    }
}

