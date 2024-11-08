using NSelene.Support.Extensions;

namespace NSelene.Tests.Integration.SharedDriver.SeleneElementSpec
{
    [TestFixture]
    public class SeleneElement_ToString_Specs : BaseTest
    {
        [Test]
        public void ShouldReflectTheFullLocatorForComposedElement()
        {
            var element = (
                SS(".parent").By(Be.Visible)[0]
                .SS(".child").ElementBy(Have.CssClass("special")) // TODO: Have.CssClass("...").And(Have.Text("...").Not)
                .S("./following-sibling::*")
            );

            var representation = element.ToString();

            Assert.That(
                representation,
                Is.EqualTo("Browser.All(.parent).By(Be.Visible)[0].All(.child).FirstBy(Have.CssClass(special)).Element(./following-sibling::*)")
            );
       }
    }
}

