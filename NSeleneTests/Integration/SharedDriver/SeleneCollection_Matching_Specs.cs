using NUnit.Framework;
using static NSelene.Selene;
using OpenQA.Selenium;

namespace NSelene.Tests.Integration.SharedDriver.SeleneElementSpec
{
    using System;
    using Harness;

    [TestFixture]
    public class SeleneCollection_Matching_Specs : BaseTest
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
            Assert.IsFalse(SS("#absent").Matching(Have.Count(2)));
            Assert.IsTrue(SS("#absent").Matching(Have.No.Count(2)));

            Assert.IsTrue(SS("#existing").Matching(Have.Count(1)));
            Assert.IsFalse(SS("#existing").Matching(Have.No.Count(1)));

            var afterCall = DateTime.Now;
            Assert.IsTrue(afterCall < beforeCall.AddSeconds(Configuration.Timeout / 2));
        }
    }
}

