﻿using System;
using NSelene;
using static NSelene.Selene;
using NUnit.Framework;

namespace NSeleneTests
{
    [TestFixture]
    public class SElementActionsTests : BaseTest
    {
        //TODO: consider not using shoulds here...

        //TODO: check "waiting InDOM/Visible" aspects 

        [Test]
        public void SElementClear()
        {
            Given.OpenedPageWithBody("<input type='text' value='ku ku'/>");
            //S("input").Clear().Should(Be.Blank()); // TODO: consider make this code still work... via tricks with explicit interface impl.
            S("input").Clear();
            S("input").Should(Be.Blank());
        }

        [Test]
        public void SElementGetAttribute()
        {
            Given.OpenedPageWithBody("<input type='text' value='ku ku'/>");
            Assert.AreEqual("text", S("input").GetAttribute("type"));
        }

        [Test]
        public void SElementGetValue()
        {
            Given.OpenedPageWithBody("<input type='text' value='ku ku'/>");
            Assert.AreEqual("ku ku", S("input").Value);
        }

        [Test]
        public void SElementGetCssValue()
        {
            Given.OpenedPageWithBody("<input type='text' value='ku ku' style='display:none'/>");
            Assert.AreEqual("none", S("input").GetCssValue("display"));
        }

        [Test]
        public void SElementIsDisplayed()
        {
            Given.OpenedPageWithBody("<input type='text' value='ku ku' style='display:none'/>");
            Assert.AreEqual(false, S("input").Displayed);
        }

        [Test]
        public void SElementIsEnabled()
        {
            Given.OpenedPageWithBody("<input type='text' value='ku ku'/>");
            Assert.AreEqual(true, S("input").Enabled);
        }

        // TODO: TBD
    }

}

