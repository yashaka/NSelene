using NUnit.Framework;
using static NSelene.Selene;
using OpenQA.Selenium;

namespace NSelene.Tests.Integration.SharedDriver.SeleneElementSpec
{
    using System;
    using Harness;

    [TestFixture]
    public class SeleneCollection_WaitUntil_Specs : BaseTest
    {

        [TearDown]
        public void TeardownTest()
        {
            Configuration.Timeout = 4;
        }
        
        [Test]
        public void ReturnsTrue_AfterWaiting_OnMatched_AfterDelayLessThanTimeout()
        {
            Configuration.Timeout = 2.000;
            var hiddenDelay = 500;
            var visibleDelay = hiddenDelay + 250;
            Given.OpenedEmptyPage();
            var beforeCall = DateTime.Now;
            Given.WithBodyTimedOut(
                "<p id='will-appear' style='display:none'>Hello!</p>", 
                hiddenDelay
            );
            Given.ExecuteScriptWithTimeout(
                "document.getElementsByTagName('p')[0].style = 'display:block';", 
                visibleDelay
            );

            var result = SS("#will-appear").WaitUntil(Have.Texts("Hello!"));
            var afterCall = DateTime.Now;

            Assert.IsTrue(result);
            Assert.IsTrue(afterCall > beforeCall.AddMilliseconds(visibleDelay));
            Assert.IsTrue(afterCall < beforeCall.AddSeconds(Configuration.Timeout));
        }
        
        [Test]
        public void ReturnsFalse_AfterWaitingTimeout_OnNotMatched_WithExceptionReason()
        {
            Configuration.Timeout = 0.75;
            var beforeCall = DateTime.Now;

            var result = SS("#absent").WaitUntil(Have.Count(2));
            var afterCall = DateTime.Now;

            Assert.IsFalse(result);
            Assert.IsTrue(afterCall >= beforeCall.AddSeconds(Configuration.Timeout));
        }
        
        [Test]
        public void ReturnsFalse_AfterWaitingTimeout_OnNotMatched()
        {
            Configuration.Timeout = 0.75;
            var beforeCall = DateTime.Now;
            Given.OpenedPageWithBody(
                "<p id='hidden' style='display:none'>Hello!</p>"
            );

            var result = SS("#hidden").WaitUntil(Have.Texts("Hello!"));
            var afterCall = DateTime.Now;

            Assert.IsFalse(result);
            Assert.IsTrue(afterCall >= beforeCall.AddSeconds(Configuration.Timeout));
        }
    }
}

