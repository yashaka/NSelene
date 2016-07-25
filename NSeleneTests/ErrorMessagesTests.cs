using System;
using NSelene;
using static NSelene.Utils;
using NUnit.Framework;
using OpenQA.Selenium;

namespace NSeleneTests
{
    [TestFixture]
    public class ErrorMessagesTests : BaseTest
    {

        [TearDown]
        public void TeardownTest()
        {
            Configuration.Timeout = 4;
        }

        [Test]
        public void SElelement_ShouldNot_FailsWhenAssertingNotVisibleForVisibleElement()
        {
            Configuration.Timeout = 0.2;
            Given.OpenedPageWithBody("<input id='new-text' type='text' value='ku ku'/>");
            Assert.Throws(Is.TypeOf(typeof(WebDriverTimeoutException))
                          .And.Message.Contains("not " + Be.Visible.GetType().Name), () => {
                S("#new-text").ShouldNot(Be.Visible);
            });
        }

        [Test]
        public void SElelement_Should_FailsWhenAssertingVisibleForHiddenElement()
        {
            Configuration.Timeout = 0.2;
            Given.OpenedPageWithBody("<input id='new-text' type='text' value='ku ku' style='display:none'/>");
            Assert.Throws(Is.TypeOf(typeof(WebDriverTimeoutException))
                          .And.Message.Not.Contains("not " + Be.Visible.GetType().Name)
                          .And.Message.Contains(Be.Visible.GetType().Name), () => {
                S("#new-text").Should(Be.Visible);
            });
        }
    }
}

