using NUnit.Framework;
using static NSelene.Selene;
using OpenQA.Selenium;

namespace NSelene.Tests.Integration.SharedDriver.SeleneElementSpec
{
    using System;
    using Harness;

    [TestFixture]
    public class SeleneElement_Matching: BaseTest
    {

        [TearDown]
        public void TeardownTest()
        {
            Configuration.Timeout = 4;
        }
        
        [Test]
        public void AllwaysReturnsBoolWithoutWaiting()
        {
            var beforeCall = DateTime.Now;
            Given.OpenedPageWithBody("<p id='existing'>Hello!</p>");

            // EXPECT
            Assert.IsFalse(S("#absent").Matching(Be.Visible));
            Assert.IsTrue(S("#absent").Matching(Be.Not.Visible));

            Assert.IsTrue(S("#existing").Matching(Be.Visible));
            Assert.IsFalse(S("#existing").Matching(Be.Not.Visible));

            var afterCall = DateTime.Now;
            Assert.IsTrue(afterCall < beforeCall.AddSeconds(Configuration.Timeout / 2));
        }
    }
}

