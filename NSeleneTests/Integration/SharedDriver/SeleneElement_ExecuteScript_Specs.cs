namespace NSelene.Tests.Integration.SharedDriver.SeleneElementSpec
{
    [TestFixture]
    public class SeleneElement_ExecuteScript_Specs : BaseTest
    {
        [Test]
        public void EchoShouldReturnArg()
        {
            Given.OpenedPageWithBody("<div/>");
            var element = S("div");

            var result = element.ExecuteScript("return args[0]", 25L);

            Assert.That(result, Is.EqualTo(25L));
        }
    }
}

