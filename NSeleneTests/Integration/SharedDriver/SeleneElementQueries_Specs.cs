namespace NSelene.Tests.Integration.SharedDriver
{
    [TestFixture]
    public class SeleneElementQueries_Specs : BaseTest
    {
        
        [Test]
        public void GetAttribute()
        {
            Given.OpenedPageWithBody("<input type='text' value='ku ku'/>");
            Assert.That(S("input").GetAttribute("type"), Is.EqualTo("text"));
        }

        [Test]
        public void GetValue()
        {
            Given.OpenedPageWithBody("<input type='text' value='ku ku'/>");
            Assert.That(S("input").Value, Is.EqualTo("ku ku"));
        }

        [Test]
        public void GetCssValue()
        {
            Given.OpenedPageWithBody("<input type='text' value='ku ku' style='display:none'/>");
            Assert.That(S("input").GetCssValue("display"), Is.EqualTo("none"));
        }

        [Test]
        public void IsDisplayed()
        {
            Given.OpenedPageWithBody("<input type='text' value='ku ku' style='display:none'/>");
            Assert.That(S("input").Displayed, Is.EqualTo(false));
        }

        [Test]
        public void IsEnabled()
        {
            Given.OpenedPageWithBody("<input type='text' value='ku ku'/>");
            Assert.That(S("input").Enabled, Is.EqualTo(true));
        }

        // TODO: TBD
    }
}

