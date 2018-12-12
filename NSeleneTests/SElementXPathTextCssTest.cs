using NUnit.Framework;
using NSelene;
using static NSelene.Selene;

namespace NSeleneTests
{
    [TestFixture]
    public class SElementXPathTextCssTest : BaseTest
    {
        [Test]
        public void SElementAlternativeCssSearch()
        {

            Given.OpenedPageWithBody("<h1 name='hello'>Hello Babe!</h1>");
            S("css = h1[name='hello']").Should(Have.Attribute("name", "hello"));
            S("h1:nth-of-type(1)").Should(Have.Text("Hello"));
        }


        [Test]
        public void SElementXpathSearch()
        {
            Given.OpenedPageWithBody("<h1>Hello Babe!</h1>");
            S("xpath = //h1").ShouldNot(Have.Text("Hello world!"));
            S("xpath = /h1[1]").ShouldNot(Have.Text("Hello world!"));

        }
        [Test]
        public void SElementTextSearch()
        {
            Given.OpenedPageWithBody("<h1>Hello Babe!</h1>");
            S("text = Hello").ShouldNot(Have.ExactText("Hello"));
            S("text = Hello Babe!").ShouldNot(Have.ExactText("Hello"));
        }
    }
}