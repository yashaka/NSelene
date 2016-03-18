using NUnit.Framework;
using NSelene;
using static NSelene.Utils;

namespace NSeleneTests.WithManagedBrowserBeforAndAfterAllTests
{
    [TestFixture]
    public class SElementConditionTests
    {

        // TODO: TBD

        [Test]
        public void SElementShouldBeVisible()
        {
            Given.OpenedPageWithBody(@"<h1 style='display:none'>ku ku</h1>");
            S("h1").ShouldNot(Be.Visible);
            When.WithBody(@"<h1 style='display:block'>ku ku</h1>");
            S("h1").Should(Be.Visible);
        }

        [Test]
        public void SElementShouldBeInDOM()
        {
            Given.OpenedEmptyPage();
            S("h1").ShouldNot(Be.InDOM);
            When.WithBody(@"<h1 style='display:none'>ku ku</h1>");
            S("h1").Should(Be.InDOM);
        }

        [Test]
        public void SElementShouldHaveText()
        {
            Given.OpenedPageWithBody("<h1>Hello Babe!</h1>");
            S("h1").ShouldNot(Have.ExactText("Hello world!"));
            When.WithBody("<h1>Hello world!</h1>");
            S("h1").Should(Have.ExactText("Hello world!"));
        }
    }
}

