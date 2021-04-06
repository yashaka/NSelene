using NUnit.Framework;
using static NSelene.Selene;

namespace NSelene.Tests.Integration.SharedDriver
{
    using Harness;

    [TestFixture]
    public class SeleneElementQueries_Specs : BaseTest
    {
        
        [Test]
        public void GetAttribute()
        {
            Given.OpenedPageWithBody("<input type='text' value='ku ku'/>");
            Assert.AreEqual("text", S("input").GetAttribute("type"));
        }

        [Test]
        public void GetValue()
        {
            Given.OpenedPageWithBody("<input type='text' value='ku ku'/>");
            Assert.AreEqual("ku ku", S("input").Value);
        }

        [Test]
        public void GetCssValue()
        {
            Given.OpenedPageWithBody("<input type='text' value='ku ku' style='display:none'/>");
            Assert.AreEqual("none", S("input").GetCssValue("display"));
        }

        [Test]
        public void IsDisplayed()
        {
            Given.OpenedPageWithBody("<input type='text' value='ku ku' style='display:none'/>");
            Assert.AreEqual(false, S("input").Displayed);
        }

        [Test]
        public void IsEnabled()
        {
            Given.OpenedPageWithBody("<input type='text' value='ku ku'/>");
            Assert.AreEqual(true, S("input").Enabled);
        }

        // TODO: TBD
    }
}

