using System;
using System.Collections.Generic;
using System.Text;
using static NSelene.Selene;
using NSelene;
using NUnit.Framework;

namespace NSeleneTests.Integration.SharedDriver
{
    using NSelene.Tests.Integration.SharedDriver.Harness;

    [TestFixture]
    public class SeleneElement_ShadowDom : BaseTest
    {
        [Test]
        public void SeleneElement_ShadowDow_ShouldFindElementInsideShadowRoot()
        {
            Given.OpenedEmptyPage();
            When.ExecuteScriptWithTimeout("const shadow = document.body.attachShadow({ mode: 'open' });shadow.innerHTML = '<span>hello from shadow</span>'; ", 10);
            var element = S("body").Shadow("span");
            var actualText = element.Text;
            Assert.IsTrue(element.Displayed);
            Assert.AreEqual("hello from shadow", actualText);
        }
    }
}
