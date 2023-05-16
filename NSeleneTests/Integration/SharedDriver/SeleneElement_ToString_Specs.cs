using NUnit.Framework;
using static NSelene.Selene;
using OpenQA.Selenium;

namespace NSelene.Tests.Integration.SharedDriver.SeleneElementSpec
{
    using System;
    using Harness;
    using NSelene.Support.Extensions;

    [TestFixture]
    public class SeleneElement_ToString_Specs : BaseTest
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
                SS(".parent").By(Be.Visible)[0]
                .SS(".child").ElementBy(Have.CssClass("special")) // TODO: Have.CssClass("...").And(Have.Text("...").Not)
                .S("./following-sibling::*")
            );

            var representation = element.ToString();

            Assert.AreEqual(
                "Browser.All(.parent).By(Be.Visible)[0].All(.child).FirstBy(Have.CssClass(special)).Element(./following-sibling::*)",
                representation
            );
       }
    }
}

