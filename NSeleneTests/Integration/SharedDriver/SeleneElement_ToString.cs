using NUnit.Framework;
using static NSelene.Selene;
using OpenQA.Selenium;

namespace NSelene.Tests.Integration.SharedDriver.SeleneElementSpec
{
    using System;
    using Harness;
    using NSelene.Support.Extensions;

    [TestFixture]
    public class SeleneElement_ToString: BaseTest
    {

        [TearDown]
        public void TeardownTest()
        {
            Configuration.Timeout = 4;
        }
        
        [Test]
        public void ShouldReflectTheFullLocatorForComposedElement()
        {
            var element = (
                SS(".parent").FilterBy(Be.Visible)[0]
                .SS(".child").FindBy(Have.CssClass("special")) // TODO: Have.CssClass("...").And(Have.Text("...").Not)
                .S("./following-sibling::*")
            );

            var representation = element.ToString();
            Console.WriteLine(representation);

            Assert.AreEqual(
                "Browser.All(.parent).By(Visible)[0].All(.child).FirstBy(has CSS class 'special').Element(./following-sibling::*)",
                representation
            );
       }
    }
}

